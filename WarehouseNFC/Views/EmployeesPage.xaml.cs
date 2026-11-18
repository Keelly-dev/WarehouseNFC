using WarehouseNFC.ViewModels;

namespace WarehouseNFC.Views;

public partial class EmployeesPage : ContentPage
{
    public EmployeesPage()
    {
        InitializeComponent();
        BindingContext = new EmployeesViewModel();
    }
}