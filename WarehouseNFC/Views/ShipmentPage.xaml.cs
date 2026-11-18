using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class ShipmentPage : ContentPage
{
    public ShipmentPage()
    {
        InitializeComponent();
        BindingContext = new ShipmentViewModel();
    }
}