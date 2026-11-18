using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class InventoryActionItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int ActualPositionId { get; set; }
        public ICommand AcceptCommand { get; set; } = null!;
        public ICommand IgnoreCommand { get; set; } = null!;
    }

    public class InventoryResultsViewModel : BaseViewModel
    {
        public static List<string> PendingOk { get; set; } = new();
        public static List<(string DisplayText, int ItemId, int ActualPositionId)> PendingMisplaced { get; set; } = new();
        public static List<(string DisplayText, int ItemId)> PendingMissing { get; set; } = new();
        public static List<string> PendingUnknown { get; set; } = new();

        public ObservableCollection<string> OkItems { get; } = new();
        public ObservableCollection<InventoryActionItem> MisplacedItems { get; } = new();
        public ObservableCollection<string> MissingItems { get; } = new();
        public ObservableCollection<InventoryActionItem> UnknownItems { get; } = new();

        // Сворачиваемые секции
        private bool _isOkExpanded = false;
        public bool IsOkExpanded
        {
            get => _isOkExpanded;
            set { _isOkExpanded = value; OnPropertyChanged(); OnPropertyChanged(nameof(OkArrow)); }
        }

        private bool _isMisplacedExpanded = false;
        public bool IsMisplacedExpanded
        {
            get => _isMisplacedExpanded;
            set { _isMisplacedExpanded = value; OnPropertyChanged(); OnPropertyChanged(nameof(MisplacedArrow)); }
        }

        private bool _isMissingExpanded = false;
        public bool IsMissingExpanded
        {
            get => _isMissingExpanded;
            set { _isMissingExpanded = value; OnPropertyChanged(); OnPropertyChanged(nameof(MissingArrow)); }
        }

        private bool _isUnknownExpanded = false;
        public bool IsUnknownExpanded
        {
            get => _isUnknownExpanded;
            set { _isUnknownExpanded = value; OnPropertyChanged(); OnPropertyChanged(nameof(UnknownArrow)); }
        }

        public string OkArrow => IsOkExpanded ? "▲" : "▼";
        public string MisplacedArrow => IsMisplacedExpanded ? "▲" : "▼";
        public string MissingArrow => IsMissingExpanded ? "▲" : "▼";
        public string UnknownArrow => IsUnknownExpanded ? "▲" : "▼";

        public string OkSummary => $"✓ На месте: {OkItems.Count}";
        public string MisplacedSummary => $"⚠️ Не на своей позиции: {MisplacedItems.Count}";
        public string MissingSummary => $"✗ Не найдено: {MissingItems.Count}";
        public string UnknownSummary => $"? Неизвестные объекты: {UnknownItems.Count}";

        public ICommand ToggleOkCommand { get; }
        public ICommand ToggleMisplacedCommand { get; }
        public ICommand ToggleMissingCommand { get; }
        public ICommand ToggleUnknownCommand { get; }
        public ICommand CloseCommand { get; }

        public InventoryResultsViewModel()
        {
            ToggleOkCommand = new Command(() => IsOkExpanded = !IsOkExpanded);
            ToggleMisplacedCommand = new Command(() => IsMisplacedExpanded = !IsMisplacedExpanded);
            ToggleMissingCommand = new Command(() => IsMissingExpanded = !IsMissingExpanded);
            ToggleUnknownCommand = new Command(() => IsUnknownExpanded = !IsUnknownExpanded);
            CloseCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            LoadResults();
        }

        private void LoadResults()
        {
            OkItems.Clear();
            foreach (var i in PendingOk) OkItems.Add(i);

            MisplacedItems.Clear();
            foreach (var (text, itemId, posId) in PendingMisplaced)
            {
                var item = new InventoryActionItem
                {
                    DisplayText = text,
                    ItemId = itemId,
                    ActualPositionId = posId
                };
                item.AcceptCommand = new Command(() => AcceptMisplaced(item));
                item.IgnoreCommand = new Command(() => MisplacedItems.Remove(item));
                MisplacedItems.Add(item);
            }

            MissingItems.Clear();
            foreach (var (text, _) in PendingMissing)
                MissingItems.Add(text);

            UnknownItems.Clear();
            foreach (var text in PendingUnknown)
            {
                var item = new InventoryActionItem
                {
                    DisplayText = text,
                    ItemId = 0
                };
                item.AcceptCommand = new Command(async () =>
                {
                    UnknownItems.Remove(item);
                    await Shell.Current.GoToAsync("receipt");
                });
                item.IgnoreCommand = new Command(() => UnknownItems.Remove(item));
                UnknownItems.Add(item);
            }
        }

        private void AcceptMisplaced(InventoryActionItem item)
        {
            var productItem = LocalDataService.ProductItems.FirstOrDefault(i => i.Id == item.ItemId);
            if (productItem != null)
                productItem.PositionId = item.ActualPositionId;
            MisplacedItems.Remove(item);
        }
    }
}