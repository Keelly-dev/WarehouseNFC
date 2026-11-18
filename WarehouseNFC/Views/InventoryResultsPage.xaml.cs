using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class InventoryResultsPage : ContentPage
{
    public InventoryResultsPage(InventoryResultsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public InventoryResultsPage() : this(new InventoryResultsViewModel())
    {
    }
}