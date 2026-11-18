using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _serverIp = string.Empty;
        public string ServerIp
        {
            get => _serverIp;
            set { _serverIp = value; OnPropertyChanged(); }
        }

        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(ServerIp))
            {
                ErrorMessage = "Введите IP-адрес сервера";
                return;
            }

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите логин и пароль";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var employee = LocalDataService.Authenticate(Login, Password);

                if (employee == null)
                {
                    ErrorMessage = "Неверный логин или пароль";
                    return;
                }

                LocalDataService.CurrentEmployee = employee;
                App.CurrentRole = employee.RoleName;
                App.CurrentFullName = employee.FullName;

                if (Shell.Current is AppShell appShell)
                    appShell.UpdateUserInfo(employee.FullName, employee.RoleName);

                await Shell.Current.GoToAsync("main");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}