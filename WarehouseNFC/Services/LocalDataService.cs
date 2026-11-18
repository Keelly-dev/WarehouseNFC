using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WarehouseNFC.Services
{
    public class LocalEmployee : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Id { get; set; }

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

        public string Password { get; set; } = string.Empty;

        private string _roleName = string.Empty;
        public string RoleName
        {
            get => _roleName;
            set { _roleName = value; OnPropertyChanged(); }
        }

        public bool IsCurrentEmployee =>
            LocalDataService.CurrentEmployee?.Id == Id;
    }

    public class LocalProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
    }

    public class LocalProductItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string PositionLabel { get; set; } = string.Empty;
        public string Status { get; set; } = "На складе";
    }

    public class LocalPosition
    {
        public int Id { get; set; }
        public int ShelfNumber { get; set; }
        public int ShelfLevel { get; set; }
        public string DisplayName => $"Стеллаж {ShelfNumber}, полка {ShelfLevel}";
    }

    public class LocalOperation
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Employee { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public List<LocalProductItem> Items { get; set; } = new();
        public List<string> InventoryResults { get; set; } = new();
    }

    public static class LocalDataService
    {
        public static List<LocalEmployee> Employees { get; } = new()
        {
            new() { Id=1, FullName="Демо-сотрудник 1", Login="demo1", Password="warehouse-demo", RoleName="Администратор" },
            new() { Id=2, FullName="Демо-сотрудник 2", Login="demo2", Password="warehouse-demo", RoleName="Кладовщик" },
            new() { Id=3, FullName="Демо-сотрудник 3", Login="demo3", Password="warehouse-demo", RoleName="Менеджер" }
        };

        public static List<LocalPosition> Positions { get; } = new()
        {
            new() { Id=1,  ShelfNumber=1, ShelfLevel=1 },
            new() { Id=2,  ShelfNumber=1, ShelfLevel=2 },
            new() { Id=3,  ShelfNumber=1, ShelfLevel=3 },
            new() { Id=4,  ShelfNumber=1, ShelfLevel=4 },
            new() { Id=5,  ShelfNumber=2, ShelfLevel=1 },
            new() { Id=6,  ShelfNumber=2, ShelfLevel=2 },
            new() { Id=7,  ShelfNumber=2, ShelfLevel=3 },
            new() { Id=8,  ShelfNumber=2, ShelfLevel=4 },
            new() { Id=9,  ShelfNumber=3, ShelfLevel=1 },
            new() { Id=10, ShelfNumber=3, ShelfLevel=2 },
            new() { Id=11, ShelfNumber=3, ShelfLevel=3 },
            new() { Id=12, ShelfNumber=3, ShelfLevel=4 }
        };

        public static List<LocalProduct> Products { get; } = new()
        {
            new() { Id=1,  Name="Насос гидравлический НГ-100", Article="НГ-100" },
            new() { Id=2,  Name="Насос гидравлический НГ-200", Article="НГ-200" },
            new() { Id=3,  Name="Насос центробежный НЦ-50",   Article="НЦ-50" },
            new() { Id=4,  Name="Насос центробежный НЦ-100",  Article="НЦ-100" },
            new() { Id=5,  Name="Насос шестерёнчатый НШ-32",  Article="НШ-32" },
            new() { Id=6,  Name="Фильтр масляный ФМ-001",     Article="ФМ-001" },
            new() { Id=7,  Name="Фильтр масляный ФМ-002",     Article="ФМ-002" },
            new() { Id=8,  Name="Фильтр воздушный ФВ-010",    Article="ФВ-010" },
            new() { Id=9,  Name="Фильтр воздушный ФВ-020",    Article="ФВ-020" },
            new() { Id=10, Name="Фильтр топливный ФТ-005",    Article="ФТ-005" },
            new() { Id=11, Name="Подшипник роликовый ПР-205",  Article="ПР-205" },
            new() { Id=12, Name="Подшипник роликовый ПР-305",  Article="ПР-305" },
            new() { Id=13, Name="Подшипник шариковый ПШ-204",  Article="ПШ-204" },
            new() { Id=14, Name="Подшипник шариковый ПШ-304",  Article="ПШ-304" },
            new() { Id=15, Name="Подшипник игольчатый ПИ-106", Article="ПИ-106" },
            new() { Id=16, Name="Редуктор цилиндрический РЦ-100", Article="РЦ-100" },
            new() { Id=17, Name="Редуктор цилиндрический РЦ-200", Article="РЦ-200" },
            new() { Id=18, Name="Редуктор червячный РЧ-050",  Article="РЧ-050" },
            new() { Id=19, Name="Редуктор червячный РЧ-100",  Article="РЧ-100" },
            new() { Id=20, Name="Редуктор планетарный РП-075", Article="РП-075" },
            new() { Id=21, Name="Муфта упругая МУ-40",        Article="МУ-40" },
            new() { Id=22, Name="Муфта упругая МУ-63",        Article="МУ-63" },
            new() { Id=23, Name="Муфта зубчатая МЗ-50",       Article="МЗ-50" },
            new() { Id=24, Name="Муфта зубчатая МЗ-80",       Article="МЗ-80" },
            new() { Id=25, Name="Муфта предохранительная МП-32", Article="МП-32" },
            new() { Id=26, Name="Вал приводной ВП-500",       Article="ВП-500" },
            new() { Id=27, Name="Вал приводной ВП-750",       Article="ВП-750" },
            new() { Id=28, Name="Вал карданный ВК-600",       Article="ВК-600" },
            new() { Id=29, Name="Вал карданный ВК-900",       Article="ВК-900" },
            new() { Id=30, Name="Вал шлицевый ВШ-400",        Article="ВШ-400" },
            new() { Id=31, Name="Шестерня цилиндрическая ШЦ-120", Article="ШЦ-120" },
            new() { Id=32, Name="Шестерня цилиндрическая ШЦ-160", Article="ШЦ-160" },
            new() { Id=33, Name="Шестерня конусная ШК-100",   Article="ШК-100" },
            new() { Id=34, Name="Шестерня конусная ШК-140",   Article="ШК-140" },
            new() { Id=35, Name="Шестерня червячная ШЧ-080",  Article="ШЧ-080" },
            new() { Id=36, Name="Клапан обратный КО-25",      Article="КО-25" },
            new() { Id=37, Name="Клапан обратный КО-40",      Article="КО-40" },
            new() { Id=38, Name="Клапан предохранительный КП-16", Article="КП-16" },
            new() { Id=39, Name="Клапан предохранительный КП-25", Article="КП-25" },
            new() { Id=40, Name="Клапан регулирующий КР-32",  Article="КР-32" },
            new() { Id=41, Name="Цилиндр гидравлический ЦГ-063", Article="ЦГ-063" },
            new() { Id=42, Name="Цилиндр гидравлический ЦГ-100", Article="ЦГ-100" },
            new() { Id=43, Name="Цилиндр пневматический ЦП-050", Article="ЦП-050" },
            new() { Id=44, Name="Цилиндр пневматический ЦП-080", Article="ЦП-080" },
            new() { Id=45, Name="Цилиндр телескопический ЦТ-125", Article="ЦТ-125" },
            new() { Id=46, Name="Манометр МН-16",             Article="МН-16" },
            new() { Id=47, Name="Манометр МН-25",             Article="МН-25" },
            new() { Id=48, Name="Датчик давления ДД-010",     Article="ДД-010" },
            new() { Id=49, Name="Датчик давления ДД-016",     Article="ДД-016" },
            new() { Id=50, Name="Датчик температуры ДТ-100",  Article="ДТ-100" },
            new() { Id=51, Name="Датчик температуры ДТ-200",  Article="ДТ-200" },
            new() { Id=52, Name="Датчик уровня ДУ-010",       Article="ДУ-010" },
            new() { Id=53, Name="Датчик уровня ДУ-020",       Article="ДУ-020" },
            new() { Id=54, Name="Датчик расхода ДР-025",      Article="ДР-025" },
            new() { Id=55, Name="Датчик расхода ДР-040",      Article="ДР-040" },
            new() { Id=56, Name="Датчик вибрации ДВ-001",     Article="ДВ-001" },
            new() { Id=57, Name="Датчик вибрации ДВ-002",     Article="ДВ-002" },
            new() { Id=58, Name="Датчик скорости ДС-100",     Article="ДС-100" },
            new() { Id=59, Name="Датчик скорости ДС-200",     Article="ДС-200" },
            new() { Id=60, Name="Датчик положения ДП-010",    Article="ДП-010" }
        };

        public static List<LocalProductItem> ProductItems { get; } = GenerateProductItems();

        private static List<LocalProductItem> GenerateProductItems()
        {
            var items = new List<LocalProductItem>();
            int id = 1;
            for (int pos = 1; pos <= 12; pos++)
            {
                var position = Positions.First(p => p.Id == pos);
                for (int i = 0; i < 5; i++)
                {
                    var productId = (pos - 1) * 5 + i + 1;
                    var product = Products.First(p => p.Id == productId);
                    items.Add(new LocalProductItem
                    {
                        Id = id++,
                        ProductId = productId,
                        ProductName = product.Name,
                        Batch = $"П-2024-{pos:D3}",
                        SerialNumber = $"СН-{id:D5}",
                        PositionId = pos,
                        PositionLabel = position.DisplayName,
                        Status = "На складе"
                    });
                }
            }
            return items;
        }

        public static List<LocalOperation> Operations { get; } = new();

        public static LocalEmployee? CurrentEmployee { get; set; }

        public static LocalEmployee? Authenticate(string login, string password)
        {
            return Employees.FirstOrDefault(e =>
                e.Login == login && e.Password == password);
        }
    }
}