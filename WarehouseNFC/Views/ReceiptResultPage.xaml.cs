using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class ReceiptResultPage : ContentPage
{
    public ReceiptResultPage()
    {
        InitializeComponent();
        BindingContext = new ReceiptResultViewModel();
    }
}