using OnlineStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync(int page, int pageSize);
        Task<int> GetTotalCount();
        Task<Product> GetProductByIdAsync(int ProductId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int ProductId);
    }
}
