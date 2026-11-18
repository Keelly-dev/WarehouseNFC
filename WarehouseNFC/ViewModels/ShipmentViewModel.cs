using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class ShipmentViewModel : BaseViewModel
    {
        private readonly INfcService _nfcService;

        public ObservableCollection<LocalProductItem> ShippedItems { get; } = new();
        public bool HasShippedItems => ShippedItems.Count > 0;

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand ScanCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand FinishCommand { get; }

        public ShipmentViewModel(INfcService nfcService)
        {
            _nfcService = nfcService;
            ScanCommand = new Command(async () => await ExecuteScanAsync());
            RemoveItemCommand = new Command<LocalProductItem>(item =>
            {
                ShippedItems.Remove(item);
                OnPropertyChanged(nameof(HasShippedItems));
            });
            FinishCommand = new Command(async () => await ExecuteFinish());
        }

        public ShipmentViewModel() : this(
            IPlatformApplication.Current?.Services.GetService<INfcService>()!)
        { }

        private async Task ExecuteScanAsync()
        {
            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                var (itemId, _) = await _nfcService.ReadTagAsync();
                var item = LocalDataService.ProductItems.FirstOrDefault(i => i.Id == itemId);

                if (item == null) { StatusMessage = "Товар не найден"; return; }
                if (item.Status == "Отгружен") { StatusMessage = "Товар уже отгружен"; return; }
                if (ShippedItems.Any(s => s.Id == item.Id)) { StatusMessage = "Товар уже в списке"; return; }

                ShippedItems.Add(item);
                OnPropertyChanged(nameof(HasShippedItems));
                StatusMessage = $"Добавлен: {item.ProductName}";
            }
            catch (Exception ex) { StatusMessage = $"Ошибка: {ex.Message}"; }
            finally { IsBusy = false; }
        }

        private async Task ExecuteFinish()
        {
            foreach (var item in ShippedItems)
                item.Status = "Отгружен";

            LocalDataService.Operations.Add(new LocalOperation
            {
                Type = "Отгрузка",
                Description = $"Отгружено товаров: {ShippedItems.Count}",
                Employee = LocalDataService.CurrentEmployee?.FullName ?? "Неизвестно",
                Items = ShippedItems.ToList()
            });

            ShipmentResultViewModel.PendingItems.Clear();
            foreach (var item in ShippedItems)
                ShipmentResultViewModel.PendingItems.Add(item);

            ShippedItems.Clear();
            OnPropertyChanged(nameof(HasShippedItems));
            StatusMessage = string.Empty;
            await Shell.Current.GoToAsync("shipmentresult");
        }
    }
}