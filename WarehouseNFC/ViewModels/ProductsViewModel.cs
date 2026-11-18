using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseNFC.Services;

namespace WarehouseNFC.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private List<LocalProduct> _allProducts = new();
        public ObservableCollection<LocalProduct> Products { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplySearch(); }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _article = string.Empty;
        public string Article
        {
            get => _article;
            set { _article = value; OnPropertyChanged(); }
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
            set { _isAdding = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand ToggleAddCommand { get; }

        public ProductsViewModel()
        {
            _allProducts = LocalDataService.Products.ToList();
            AddCommand = new Command(() => ExecuteAdd());
            ToggleAddCommand = new Command(() => IsAdding = !IsAdding);
            ApplySearch();
        }

        private void ApplySearch()
        {
            Products.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allProducts
                : _allProducts.Where(p =>
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Article.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            foreach (var p in filtered) Products.Add(p);
        }

        private void ExecuteAdd()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                StatusMessage = "Введите наименование";
                IsError = true;
                return;
            }

            var product = new LocalProduct
            {
                Id = LocalDataService.Products.Count + 1,
                Name = Name,
                Article = Article
            };

            LocalDataService.Products.Add(product);
            _allProducts = LocalDataService.Products.ToList();
            ApplySearch();

            StatusMessage = "Продукция добавлена";
            IsError = false;
            Name = string.Empty;
            Article = string.Empty;
            IsAdding = false;
        }
    }
}