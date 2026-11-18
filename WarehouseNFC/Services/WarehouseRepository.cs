using Microsoft.EntityFrameworkCore;
using WarehouseNFC.Data.Models;

namespace WarehouseNFC.Services
{
    public class WarehouseRepository
    {
        private readonly WarehouseNfcContext _context;

        public WarehouseRepository(WarehouseNfcContext context)
        {
            _context = context;
        }

        // Сотрудники
        public async Task<Employee?> GetEmployeeByLoginAsync(string login, string passwordHash)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Login == login && e.PasswordHash == passwordHash);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Role)
                .ToListAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        // Товары
        public async Task<ProductItem?> GetProductItemByIdAsync(int itemId)
        {
            return await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(p => p.ItemId == itemId);
        }

        public async Task<List<ProductItem>> GetAllProductItemsAsync()
        {
            return await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.Position)
                .ToListAsync();
        }

        public async Task<List<ProductItem>> GetProductItemsByPositionAsync(int positionId)
        {
            return await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.Position)
                .Where(p => p.PositionId == positionId && p.Status == "На складе")
                .ToListAsync();
        }

        public async Task<ProductItem> AddProductItemAsync(ProductItem item)
        {
            _context.ProductItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateProductItemAsync(ProductItem item)
        {
            _context.ProductItems.Update(item);
            await _context.SaveChangesAsync();
        }

        // NFC-метки
        public async Task<NfcTag?> GetNfcTagByTagIdAsync(string tagId)
        {
            return await _context.NfcTags
                .Include(t => t.Item)
                .Include(t => t.Position)
                .FirstOrDefaultAsync(t => t.TagId == tagId);
        }

        public async Task AddNfcTagAsync(NfcTag tag)
        {
            _context.NfcTags.Add(tag);
            await _context.SaveChangesAsync();
        }

        // Операции
        public async Task AddOperationAsync(Operation operation, List<OperationItem> items)
        {
            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();
            foreach (var item in items)
            {
                item.OperationId = operation.OperationId;
                _context.OperationItems.Add(item);
            }
            await _context.SaveChangesAsync();
        }

        // Позиции хранения
        public async Task<List<StoragePosition>> GetAllPositionsAsync()
        {
            return await _context.StoragePositions.ToListAsync();
        }

        // Номенклатура
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Роли
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}