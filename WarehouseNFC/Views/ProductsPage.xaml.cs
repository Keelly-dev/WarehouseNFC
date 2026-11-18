using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class ProductsPage : ContentPage
{
    public ProductsPage()
    {
        InitializeComponent();
        BindingContext = new ProductsViewModel();
    }
}