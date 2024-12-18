using OnlineStore.IRepository;
using OnlineStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;


        public CategoryController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActionResult> DisplayCategories()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var categories = await _repository.GetAllCategoriesAsync();

                if (categories != null && categories.Any())
                {
                    return View(categories);
                }
                else
                {
                    ViewBag.ErrorMessage = "No categories found.";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching categories.";
                return View("Eror");
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
                var isDuplicate = await _repository.GetCategoryByNameAsync(category.CategoryName);

                if (isDuplicate != null)
                {
                    ModelState.AddModelError("", "Duplicate category not allowed");
                    return View("Error", category);
                }


                await _repository.AddCategoryAsync(category);

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
                Category category = await _repository.GetCategoryByIdAsync(categoryId);
                if (category == null)
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
                if (category.CategoryId >= 0)
                {
                    await _repository.UpdateCategoryAsync(category);
                    return RedirectToAction("DisplayCategories");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Category ID.");
                    return View(category);
                }
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
                var category = await _repository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return HttpNotFound("Category not found.");
                }

                await _repository.DeleteCategoryAsync(categoryId);
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