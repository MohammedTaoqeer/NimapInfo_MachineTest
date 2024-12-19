using OnlineStore.IRepository;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext _storeDbContext;
        public CategoryRepository(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _storeDbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int CategoryId)
        {
            return await _storeDbContext.Categories.FindAsync(CategoryId);
        }

        public async Task AddCategoryAsync(Category category)
        {
            bool categoryExists = await _storeDbContext.Categories
                                 .AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName);

            if (categoryExists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            _storeDbContext.Categories.Add(category);
            await _storeDbContext.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _storeDbContext.Entry(category).State = EntityState.Modified;
            await _storeDbContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int CategoryId)
        {
            var data = await _storeDbContext.Categories
                        .Where(c => c.CategoryId == CategoryId)
                        .FirstOrDefaultAsync();

            if (data != null)
            {
                _storeDbContext.Categories.Remove(data);
                await _storeDbContext.SaveChangesAsync();
            }
        }

        public async Task<Category> GetCategoryByNameAsync(string Name)
        {
            return await _storeDbContext.Categories.FirstOrDefaultAsync(s => s.CategoryName == Name);
        }
    }
}