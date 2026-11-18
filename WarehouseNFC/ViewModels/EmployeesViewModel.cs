using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class LocalRole
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public class EmployeesViewModel : BaseViewModel
    {
        private List<LocalEmployee> _allEmployees = new();
        public ObservableCollection<LocalEmployee> Employees { get; } = new();
        public ObservableCollection<LocalRole> Roles { get; } = new()
        {
            new() { RoleName = "Кладовщик" },
            new() { RoleName = "Менеджер" },
            new() { RoleName = "Администратор" }
        };

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplySearch(); }
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
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

        private LocalRole? _selectedRole;
        public LocalRole? SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError;
            set { _isError = value; OnPropertyChanged(); }
        }

        private bool _isAdding;
        public bool IsAdding
        {
            get => _isAdding;
            set { _isAdding = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNormal)); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNormal)); }
        }

        public bool IsNormal => !IsAdding && !IsEditing;

        private LocalEmployee? _editingEmployee;
        public LocalEmployee? EditingEmployee
        {
            get => _editingEmployee;
            set { _editingEmployee = value; OnPropertyChanged(); }
        }

        private string _editFullName = string.Empty;
        public string EditFullName
        {
            get => _editFullName;
            set { _editFullName = value; OnPropertyChanged(); }
        }

        private string _editLogin = string.Empty;
        public string EditLogin
        {
            get => _editLogin;
            set { _editLogin = value; OnPropertyChanged(); }
        }

        private LocalRole? _editSelectedRole;
        public LocalRole? EditSelectedRole
        {
            get => _editSelectedRole;
            set { _editSelectedRole = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand ToggleAddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteCommand { get; }

        public EmployeesViewModel()
        {
            _allEmployees = LocalDataService.Employees.ToList();

            AddCommand = new Command(() => ExecuteAdd());

            ToggleAddCommand = new Command(() =>
            {
                IsAdding = !IsAdding;
                IsEditing = false;
            });

            EditCommand = new Command<LocalEmployee>(employee =>
            {
                EditingEmployee = employee;
                EditFullName = employee.FullName;
                EditLogin = employee.Login;
                EditSelectedRole = Roles.FirstOrDefault(r => r.RoleName == employee.RoleName);
                IsEditing = true;
                IsAdding = false;
            });

            SaveEditCommand = new Command(() => ExecuteSaveEdit());

            CancelEditCommand = new Command(() =>
            {
                IsEditing = false;
                EditingEmployee = null;
            });

            DeleteCommand = new Command<LocalEmployee>(async employee =>
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Удаление",
                    $"Удалить сотрудника {employee.FullName}?",
                    "Удалить",
                    "Отмена");

                if (!confirm) return;

                LocalDataService.Employees.Remove(employee);
                _allEmployees = LocalDataService.Employees.ToList();
                ApplySearch();
                IsEditing = false;
                EditingEmployee = null;
                StatusMessage = "Сотрудник удалён";
                IsError = false;
            });

            ApplySearch();
        }

        private void ApplySearch()
        {
            Employees.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allEmployees
                : _allEmployees.Where(e =>
                    e.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.Login.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.RoleName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            foreach (var e in filtered) Employees.Add(e);
        }

        private void ExecuteAdd()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Login) ||
                string.IsNullOrWhiteSpace(Password) || SelectedRole == null)
            {
                StatusMessage = "Заполните все поля";
                IsError = true;
                return;
            }

            var employee = new LocalEmployee
            {
                Id = LocalDataService.Employees.Count + 1,
                FullName = FullName,
                Login = Login,
                Password = Password,
                RoleName = SelectedRole.RoleName
            };

            LocalDataService.Employees.Add(employee);
            _allEmployees = LocalDataService.Employees.ToList();
            ApplySearch();

            StatusMessage = "Сотрудник добавлен";
            IsError = false;
            FullName = string.Empty;
            Login = string.Empty;
            Password = string.Empty;
            SelectedRole = null;
            IsAdding = false;
        }

        private void ExecuteSaveEdit()
        {
            if (EditingEmployee == null) return;

            if (string.IsNullOrWhiteSpace(EditFullName) || string.IsNullOrWhiteSpace(EditLogin) ||
                EditSelectedRole == null)
            {
                StatusMessage = "Заполните все поля";
                IsError = true;
                return;
            }

            // Обновляем напрямую в LocalDataService
            var target = LocalDataService.Employees.FirstOrDefault(e => e.Id == EditingEmployee.Id);
            if (target != null)
            {
                target.FullName = EditFullName;
                target.Login = EditLogin;
                target.RoleName = EditSelectedRole.RoleName;
            }

            _allEmployees = LocalDataService.Employees.ToList();
            ApplySearch();

            StatusMessage = "Данные сохранены";
            IsError = false;
            IsEditing = false;
            EditingEmployee = null;
        }
    }
}