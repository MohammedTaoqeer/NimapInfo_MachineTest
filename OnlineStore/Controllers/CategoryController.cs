using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly StoreDbContext _dbContext;

        
        public CategoryController()
        {
            _dbContext = new StoreDbContext();
        }

        public async Task<ActionResult> DisplayCategories()
        {
            try
            {
                var categories = await _dbContext.Categories.ToListAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching categories.";
                return View("Error");
            }
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                 _dbContext.Categories.Add(category);
                 await _dbContext.SaveChangesAsync();
                return RedirectToAction("DisplayCategories");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the category.";
                return View("Error");
            }
        }

        public async Task<ActionResult> EditCategory(int categoryId)
        {
            try
            {
                 Category category = await _dbContext.Categories.FindAsync(categoryId);
                 if(category == null)
                 {
                    return HttpNotFound("Category Not found");
                 }

                 return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while editing the category.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("EditCategory", category);
            }

            try
            {
                _dbContext.Entry(category).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("DisplayCategories");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the category.";
                return View("Error");
            }
        }

        public async Task<ActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await _dbContext.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return HttpNotFound("Category not found.");
                }

                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("DisplayCategories");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the category.";
                return View("Error");
            }
        }
    }
}