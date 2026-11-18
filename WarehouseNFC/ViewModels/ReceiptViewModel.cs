using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class ReceiptViewModel : BaseViewModel
    {
        public static string SavedBatch { get; set; } = string.Empty;

        public ObservableCollection<LocalPosition> Positions { get; } = new();
        public ObservableCollection<LocalProduct> ProductSuggestions { get; } = new();
        public ObservableCollection<LocalProductItem> ReceivedItems { get; } = new();

        public bool HasReceivedItems => ReceivedItems.Count > 0;
        public bool IsBatchFixed => !string.IsNullOrWhiteSpace(FixedBatch);
        public string ReceivedSummary => $"Принято товаров: {ReceivedItems.Count} (партия: {FixedBatch})";

        private string _fixedBatch = string.Empty;
        public string FixedBatch
        {
            get => _fixedBatch;
            set
            {
                _fixedBatch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBatchFixed));
                OnPropertyChanged(nameof(ReceivedSummary));
            }
        }

        private LocalProduct? _selectedProduct;
        public LocalProduct? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        private LocalPosition? _selectedPosition;
        public LocalPosition? SelectedPosition
        {
            get => _selectedPosition;
            set { _selectedPosition = value; OnPropertyChanged(); }
        }

        private string _productSearch = string.Empty;
        public string ProductSearch
        {
            get => _productSearch;
            set { _productSearch = value; OnPropertyChanged(); ApplyProductSearch(); }
        }

        private bool _showProductSuggestions;
        public bool ShowProductSuggestions
        {
            get => _showProductSuggestions;
            set { _showProductSuggestions = value; OnPropertyChanged(); }
        }

        private string _batch = string.Empty;
        public string Batch
        {
            get => _batch;
            set { _batch = value; OnPropertyChanged(); }
        }

        private string _serialNumber = string.Empty;
        public string SerialNumber
        {
            get => _serialNumber;
            set { _serialNumber = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError;
            set { _isError = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand FinishCommand { get; }

        public ReceiptViewModel()
        {
            SaveCommand = new Command(async () => await ExecuteSave());
            SelectProductCommand = new Command<LocalProduct>(product =>
            {
                SelectedProduct = product;
                ProductSearch = product.Name;
                ShowProductSuggestions = false;
            });
            RemoveItemCommand = new Command<LocalProductItem>(item =>
            {
                ReceivedItems.Remove(item);
                OnPropertyChanged(nameof(HasReceivedItems));
                OnPropertyChanged(nameof(ReceivedSummary));
            });
            FinishCommand = new Command(async () => await ExecuteFinish());
            LoadData();

            if (!string.IsNullOrWhiteSpace(SavedBatch))
                FixedBatch = SavedBatch;
        }

        private void LoadData()
        {
            foreach (var p in LocalDataService.Positions)
                Positions.Add(p);
        }

        private void ApplyProductSearch()
        {
            ProductSuggestions.Clear();
            if (string.IsNullOrWhiteSpace(ProductSearch))
            {
                ShowProductSuggestions = false;
                SelectedProduct = null;
                return;
            }

            var filtered = LocalDataService.Products
                .Where(p => p.Name.Contains(ProductSearch, StringComparison.OrdinalIgnoreCase) ||
                            p.Article.Contains(ProductSearch, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            foreach (var p in filtered) ProductSuggestions.Add(p);
            ShowProductSuggestions = filtered.Count > 0;
        }

        private async Task ExecuteSave()
        {
            if (SelectedProduct == null || SelectedPosition == null)
            {
                StatusMessage = "Выберите продукцию и позицию хранения";
                IsError = true;
                return;
            }

            var batchToUse = IsBatchFixed ? FixedBatch : Batch;

            if (string.IsNullOrWhiteSpace(batchToUse))
            {
                StatusMessage = "Введите номер партии";
                IsError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(SerialNumber))
            {
                StatusMessage = "Введите серийный номер";
                IsError = true;
                return;
            }

            if (!IsBatchFixed)
                FixedBatch = Batch;

            var newItem = new LocalProductItem
            {
                Id = LocalDataService.ProductItems.Count + 1,
                ProductId = SelectedProduct.Id,
                ProductName = SelectedProduct.Name,
                Batch = batchToUse,
                SerialNumber = SerialNumber,
                PositionId = SelectedPosition.Id,
                PositionLabel = SelectedPosition.DisplayName,
                Status = "На складе"
            };

            LocalDataService.ProductItems.Add(newItem);
            ReceivedItems.Add(newItem);
            OnPropertyChanged(nameof(HasReceivedItems));
            OnPropertyChanged(nameof(ReceivedSummary));
            StatusMessage = $"Добавлен: {SelectedProduct.Name}";
            IsError = false;
            Batch = string.Empty;
            SerialNumber = string.Empty;
            ProductSearch = string.Empty;
            SelectedProduct = null;
            SelectedPosition = null;
        }

        private async Task ExecuteFinish()
        {
            LocalDataService.Operations.Add(new LocalOperation
            {
                Type = $"Приёмка (партия: {FixedBatch})",
                Description = $"Принято товаров: {ReceivedItems.Count}",
                Employee = LocalDataService.CurrentEmployee?.FullName ?? "Неизвестно",
                Batch = FixedBatch,
                Items = ReceivedItems.ToList()
            });

            ReceiptResultViewModel.PendingItems.Clear();
            foreach (var item in ReceivedItems)
                ReceiptResultViewModel.PendingItems.Add(item);

            ReceiptResultViewModel.PendingBatch = FixedBatch;

            SavedBatch = string.Empty;
            ReceivedItems.Clear();
            FixedBatch = string.Empty;
            OnPropertyChanged(nameof(HasReceivedItems));
            OnPropertyChanged(nameof(ReceivedSummary));
            StatusMessage = string.Empty;
            await Shell.Current.GoToAsync("receiptresult");
        }
    }
}