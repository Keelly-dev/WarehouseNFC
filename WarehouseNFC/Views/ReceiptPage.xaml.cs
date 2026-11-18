using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class ReceiptPage : ContentPage
{
    public ReceiptPage()
    {
        InitializeComponent();
        BindingContext = new ReceiptViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReceiptViewModel vm && !string.IsNullOrWhiteSpace(ReceiptViewModel.SavedBatch))
        {
            vm.FixedBatch = ReceiptViewModel.SavedBatch;
            ReceiptViewModel.SavedBatch = string.Empty;
        }
    }
}