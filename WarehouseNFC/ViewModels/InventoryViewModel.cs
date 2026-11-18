using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class InventoryResultItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public string ResultIcon { get; set; } = string.Empty;
        public Color ResultColor { get; set; } = Colors.Gray;
        public int ItemId { get; set; }
        public int ActualPositionId { get; set; }
        public bool IsMissing { get; set; }
        public bool IsUnknown { get; set; }
    }

    public class ScannedItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Color IconColor { get; set; } = Colors.Gray;
        public bool IsOk { get; set; }
        public bool IsMisplaced { get; set; }
        public bool IsUnknown { get; set; }
        public int ItemId { get; set; }
    }

    public class InventoryViewModel : BaseViewModel
    {
        private readonly INfcService _nfcService;

        private List<LocalPosition> _allPositions = new();
        private int _currentPositionIndex = 0;
        private List<LocalProductItem> _expectedItems = new();

        public ObservableCollection<ScannedItem> ScannedItems { get; } = new();
        public ObservableCollection<InventoryResultItem> FinalResults { get; } = new();

        private LocalPosition? _currentPosition;
        public LocalPosition? CurrentPosition
        {
            get => _currentPosition;
            set { _currentPosition = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentPositionLabel)); }
        }

        public string CurrentPositionLabel => CurrentPosition == null ? string.Empty :
            $"Стеллаж {CurrentPosition.ShelfNumber}, полка {CurrentPosition.ShelfLevel}";

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isFinished;
        public bool IsFinished
        {
            get => _isFinished;
            set { _isFinished = value; OnPropertyChanged(); }
        }

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            set { _isStarted = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand NextCommand { get; }

        public InventoryViewModel(INfcService nfcService)
        {
            _nfcService = nfcService;
            StartCommand = new Command(() => ExecuteStart());
            ScanCommand = new Command(async () => await ExecuteScanAsync());
            NextCommand = new Command(() => ExecuteNext());
        }

        public InventoryViewModel() : this(
            IPlatformApplication.Current?.Services.GetService<INfcService>()!)
        { }

        private void ExecuteStart()
        {
            _allPositions = LocalDataService.Positions.ToList();
            _currentPositionIndex = 0;
            IsStarted = true;
            IsFinished = false;
            FinalResults.Clear();
            LoadCurrentPosition();
        }

        private void LoadCurrentPosition()
        {
            if (_currentPositionIndex >= _allPositions.Count)
            {
                FinishInventory();
                return;
            }

            CurrentPosition = _allPositions[_currentPositionIndex];
            ScannedItems.Clear();
            _expectedItems = LocalDataService.ProductItems
                .Where(i => i.PositionId == CurrentPosition.Id && i.Status == "На складе")
                .ToList();
            StatusMessage = "Отсканируйте все товары на позиции";

            if (_nfcService is MockNfcService mock)
                mock.ResetIndex();
        }

        private async Task ExecuteScanAsync()
        {
            IsBusy = true;
            try
            {
                var (itemId, tagPositionId) = await _nfcService.ReadTagAsync(CurrentPosition?.Id ?? 0);
                var item = LocalDataService.ProductItems.FirstOrDefault(i => i.Id == itemId);

                if (item == null)
                {
                    ScannedItems.Add(new ScannedItem
                    {
                        DisplayText = "Неизвестный объект",
                        Icon = "✗",
                        IconColor = Colors.Red,
                        IsOk = false,
                        IsMisplaced = false,
                        IsUnknown = true,
                        ItemId = 0
                    });
                    return;
                }

                var displayName = item.ProductName;

                if (tagPositionId == CurrentPosition?.Id)
                {
                    ScannedItems.Add(new ScannedItem
                    {
                        DisplayText = displayName,
                        Icon = "✓",
                        IconColor = Color.FromArgb("#43A047"),
                        IsOk = true,
                        IsMisplaced = false,
                        IsUnknown = false,
                        ItemId = item.Id
                    });
                }
                else
                {
                    var tagPosition = LocalDataService.Positions
                        .FirstOrDefault(p => p.Id == tagPositionId);
                    var tagPositionLabel = tagPosition != null
                        ? $"Стеллаж {tagPosition.ShelfNumber}, полка {tagPosition.ShelfLevel}"
                        : "неизвестная позиция";

                    ScannedItems.Add(new ScannedItem
                    {
                        DisplayText = $"{displayName} — в базе данных находится на: {tagPositionLabel}",
                        Icon = "⚠️",
                        IconColor = Colors.Transparent,
                        IsOk = false,
                        IsMisplaced = true,
                        IsUnknown = false,
                        ItemId = item.Id
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecuteNext()
        {
            var scannedOkIds = ScannedItems
                .Where(s => s.IsOk)
                .Select(s => s.ItemId)
                .ToList();

            var scannedMisplacedIds = ScannedItems
                .Where(s => s.IsMisplaced)
                .Select(s => s.ItemId)
                .ToList();

            foreach (var expected in _expectedItems)
            {
                if (scannedOkIds.Contains(expected.Id))
                {
                    FinalResults.Add(new InventoryResultItem
                    {
                        DisplayText = $"{expected.ProductName} — {CurrentPositionLabel}",
                        ResultIcon = "✓",
                        ResultColor = Color.FromArgb("#43A047"),
                        ItemId = expected.Id,
                        IsMissing = false,
                        IsUnknown = false
                    });
                }
                else if (!scannedMisplacedIds.Contains(expected.Id))
                {
                    // Товар не отсканирован вообще — не найден
                    FinalResults.Add(new InventoryResultItem
                    {
                        DisplayText = $"{expected.ProductName} — не обнаружен на {CurrentPositionLabel}",
                        ResultIcon = "✗",
                        ResultColor = Colors.Red,
                        ItemId = expected.Id,
                        IsMissing = true,
                        IsUnknown = false
                    });
                }
            }

            foreach (var scanned in ScannedItems.Where(s => s.IsMisplaced))
            {
                FinalResults.Add(new InventoryResultItem
                {
                    DisplayText = scanned.DisplayText,
                    ResultIcon = "⚠️",
                    ResultColor = Color.FromArgb("#F57C00"),
                    ItemId = scanned.ItemId,
                    ActualPositionId = CurrentPosition?.Id ?? 0,
                    IsMissing = false,
                    IsUnknown = false
                });
            }

            foreach (var scanned in ScannedItems.Where(s => s.IsUnknown))
            {
                FinalResults.Add(new InventoryResultItem
                {
                    DisplayText = "Неизвестный объект",
                    ResultIcon = "?",
                    ResultColor = Colors.Gray,
                    ItemId = 0,
                    IsMissing = false,
                    IsUnknown = true
                });
            }

            _currentPositionIndex++;
            LoadCurrentPosition();
        }

        private void FinishInventory()
        {
            LocalDataService.Operations.Add(new LocalOperation
            {
                Type = "Инвентаризация",
                Description = $"Проверено товаров: {FinalResults.Count}",
                Employee = LocalDataService.CurrentEmployee?.FullName ?? "Неизвестно",
                InventoryResults = FinalResults.Select(r => $"{r.ResultIcon} {r.DisplayText}").ToList()
            });

            InventoryResultsViewModel.PendingOk = FinalResults
                .Where(r => r.ResultIcon == "✓")
                .Select(r => r.DisplayText)
                .ToList();

            InventoryResultsViewModel.PendingMisplaced = FinalResults
                .Where(r => r.ResultIcon == "⚠️")
                .Select(r => (r.DisplayText, r.ItemId, r.ActualPositionId))
                .ToList();

            InventoryResultsViewModel.PendingMissing = FinalResults
                .Where(r => r.IsMissing)
                .Select(r => (r.DisplayText, r.ItemId))
                .ToList();

            InventoryResultsViewModel.PendingUnknown = FinalResults
                .Where(r => r.IsUnknown)
                .Select(r => r.DisplayText)
                .ToList();

            IsStarted = false;
            CurrentPosition = null;
            FinalResults.Clear();

            Shell.Current.GoToAsync("inventoryresults");
        }
    }
}