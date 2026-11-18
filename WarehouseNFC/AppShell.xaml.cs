using WarehouseNFC.Views;

namespace WarehouseNFC;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("main", typeof(MainPage));
        Routing.RegisterRoute("receipt", typeof(ReceiptPage));
        Routing.RegisterRoute("shipment", typeof(ShipmentPage));
        Routing.RegisterRoute("inventory", typeof(InventoryPage));
        Routing.RegisterRoute("products", typeof(ProductsPage));
        Routing.RegisterRoute("employees", typeof(EmployeesPage));
        Routing.RegisterRoute("inventoryresults", typeof(InventoryResultsPage));
        Routing.RegisterRoute("receiptresult", typeof(ReceiptResultPage));
        Routing.RegisterRoute("shipmentresult", typeof(ShipmentResultPage));
        Routing.RegisterRoute("history", typeof(HistoryPage));

        Navigated += OnNavigated;
    }

    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (CurrentPage != null && PageTitleLabel != null)
            PageTitleLabel.Text = CurrentPage.Title;
    }

    public void UpdateUserInfo(string fullName, string roleName) { }
    public void ClearUserInfo() { }
}