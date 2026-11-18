using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class ShipmentResultViewModel : BaseViewModel
    {
        public static List<LocalProductItem> PendingItems { get; set; } = new();

        public ObservableCollection<LocalProductItem> ShippedItems { get; } = new();

        public ICommand ShipMoreCommand { get; }
        public ICommand GoHomeCommand { get; }

        public ShipmentResultViewModel()
        {
            foreach (var item in PendingItems)
                ShippedItems.Add(item);

            PendingItems.Clear();

            ShipMoreCommand = new Command(async () =>
                await Shell.Current.GoToAsync(".."));

            GoHomeCommand = new Command(async () =>
                await Shell.Current.GoToAsync("main"));
        }
    }
}