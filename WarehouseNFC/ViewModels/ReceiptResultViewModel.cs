using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class ReceiptResultViewModel : BaseViewModel
    {
        public static List<LocalProductItem> PendingItems { get; set; } = new();
        public static string PendingBatch { get; set; } = string.Empty;

        public ObservableCollection<LocalProductItem> ReceivedItems { get; } = new();

        public string Batch { get; private set; } = string.Empty;
        public string Summary => $"Принято товаров: {ReceivedItems.Count}";
        public string Title => $"Приёмка партии {Batch} завершена";

        public ICommand ReceiveMoreCommand { get; }
        public ICommand GoHomeCommand { get; }

        public ReceiptResultViewModel()
        {
            foreach (var item in PendingItems)
                ReceivedItems.Add(item);

            Batch = PendingBatch;

            PendingItems.Clear();
            PendingBatch = string.Empty;

            ReceiveMoreCommand = new Command(async () =>
            {
                ReceiptViewModel.SavedBatch = Batch;
                await Shell.Current.GoToAsync("..");
            });

            GoHomeCommand = new Command(async () =>
                await Shell.Current.GoToAsync("main"));
        }
    }
}