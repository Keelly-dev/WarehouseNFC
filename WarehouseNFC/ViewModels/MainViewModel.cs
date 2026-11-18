using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _roleName = string.Empty;
        public string RoleName
        {
            get => _roleName;
            set
            {
                _roleName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanReceive));
                OnPropertyChanged(nameof(CanShip));
                OnPropertyChanged(nameof(CanInventory));
                OnPropertyChanged(nameof(CanManageProducts));
                OnPropertyChanged(nameof(CanManageEmployees));
                OnPropertyChanged(nameof(CanViewHistory));
            }
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public bool CanReceive => RoleName == "Кладовщик" || RoleName == "Администратор";
        public bool CanShip => RoleName == "Кладовщик" || RoleName == "Администратор";
        public bool CanInventory => true;
        public bool CanManageProducts => RoleName == "Менеджер" || RoleName == "Администратор";
        public bool CanManageEmployees => RoleName == "Администратор";
        public bool CanViewHistory => RoleName != "Кладовщик";

        public ICommand ReceiptCommand { get; }
        public ICommand ShipmentCommand { get; }
        public ICommand InventoryCommand { get; }
        public ICommand ProductsCommand { get; }
        public ICommand EmployeesCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            RoleName = App.CurrentRole;
            FullName = App.CurrentFullName;

            ReceiptCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("receipt"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            ShipmentCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("shipment"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            InventoryCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("inventory"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            ProductsCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("products"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            EmployeesCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("employees"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            HistoryCommand = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("history"); }
                catch (Exception ex) { await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK"); }
            });

            LogoutCommand = new Command(async () =>
            {
                App.CurrentRole = string.Empty;
                App.CurrentFullName = string.Empty;
                LocalDataService.CurrentEmployee = null;
                if (Shell.Current is AppShell appShell)
                    appShell.ClearUserInfo();
                await Shell.Current.GoToAsync("//login");
            });
        }
    }
}