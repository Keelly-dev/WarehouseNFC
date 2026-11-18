using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        BindingContext = new HistoryViewModel();
    }
}