using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<LocalOperation> Operations { get; } = new();

        private LocalOperation? _selectedOperation;
        public LocalOperation? SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                _selectedOperation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(HasItems));
                OnPropertyChanged(nameof(HasInventoryResults));
                OnPropertyChanged(nameof(IsReceiptOperation));
                OnPropertyChanged(nameof(IsShipmentOperation));
                OnPropertyChanged(nameof(IsInventoryOperation));
                OnPropertyChanged(nameof(ItemsHeader));

                // Сбрасываем секции при выборе новой операции
                IsInventoryExpanded = false;
            }
        }

        public bool HasSelection => SelectedOperation != null;
        public bool HasItems => SelectedOperation?.Items?.Count > 0;
        public bool HasInventoryResults => SelectedOperation?.InventoryResults?.Count > 0;
        public bool IsReceiptOperation => SelectedOperation?.Type.StartsWith("Приёмка") ?? false;
        public bool IsShipmentOperation => SelectedOperation?.Type.StartsWith("Отгрузка") ?? false;
        public bool IsInventoryOperation => SelectedOperation?.Type.StartsWith("Инвентаризация") ?? false;

        public string ItemsHeader
        {
            get
            {
                if (SelectedOperation == null) return string.Empty;
                if (IsReceiptOperation)
                    return $"Список принятых товаров (партия: {SelectedOperation.Batch})";
                if (IsShipmentOperation)
                    return $"Список отгруженных товаров ({SelectedOperation.DateTime:dd.MM.yyyy HH:mm})";
                if (IsInventoryOperation)
                    return $"Результаты инвентаризации ({SelectedOperation.DateTime:dd.MM.yyyy HH:mm})";
                return "Товары";
            }
        }

        // Сворачиваемые секции для инвентаризации
        private bool _isInventoryExpanded = false;
        public bool IsInventoryExpanded
        {
            get => _isInventoryExpanded;
            set { _isInventoryExpanded = value; OnPropertyChanged(); OnPropertyChanged(nameof(InventoryArrow)); }
        }

        public string InventoryArrow => IsInventoryExpanded ? "▲" : "▼";

        public ICommand SelectCommand { get; }
        public ICommand CloseDetailCommand { get; }
        public ICommand ToggleInventoryCommand { get; }

        public HistoryViewModel()
        {
            foreach (var op in LocalDataService.Operations.AsEnumerable().Reverse())
                Operations.Add(op);

            SelectCommand = new Command<LocalOperation>(op => SelectedOperation = op);
            CloseDetailCommand = new Command(() => SelectedOperation = null);
            ToggleInventoryCommand = new Command(() => IsInventoryExpanded = !IsInventoryExpanded);
        }
    }
}