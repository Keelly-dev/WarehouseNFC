using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class ShipmentResultPage : ContentPage
{
    public ShipmentResultPage()
    {
        InitializeComponent();
        BindingContext = new ShipmentResultViewModel();
    }
}