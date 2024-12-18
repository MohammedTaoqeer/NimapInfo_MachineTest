using OnlineStore.IRepository;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{

    public class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext _storeDbContext;
        public ProductRepository(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }

        public async Task<List<Product>> GetAllProductAsync(int page, int pageSize)
        {
            var query = _storeDbContext.Products
                             .Include(p => p.Category)
                             .Where(p => p.Discontinued == false);


            query = query.OrderBy(p => p.ProductId);

            var products = await query
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            return products;

        }


        public async Task<int> GetTotalCount()
        {
            return await _storeDbContext.Products
                                             .Where(p => p.Discontinued == false)
                                             .CountAsync();
        }

        public async Task<Product> GetProductByIdAsync(int ProductId)
        {

            return await _storeDbContext.Products
                             .Include(p => p.Category)
                             .Where(p => p.ProductId == ProductId && p.Discontinued == false)
                             .FirstOrDefaultAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _storeDbContext.Products.Add(product);
            await _storeDbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _storeDbContext.Entry(product).State = EntityState.Modified;
            await _storeDbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int ProductId)
        {
            var data = await _storeDbContext.Products
                                   .FindAsync(ProductId);

            if (data != null)
            {
                data.Discontinued = true;
                _storeDbContext.Entry(data).State = EntityState.Modified;
                await _storeDbContext.SaveChangesAsync();
            }
        }
    }
}