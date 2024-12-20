﻿using OnlineStore.IRepository;
using OnlineStore.Models;
using OnlineStore.Services;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace OnlineStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileUploadService _fileUploadService;
        public ProductController(IProductRepository repository,
                                 ICategoryRepository categoryRepository,
                                 IFileUploadService fileUploadService)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<ActionResult> DisplayProducts(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var products = await _repository.GetAllProductAsync(page, pageSize);

                var totalProducts = await _repository.GetTotalCount();

                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);


                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }
        }

        public async Task<ActionResult> DisplayProduct(int ProductId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var product = await _repository.GetProductByIdAsync(ProductId);

                if (product != null)
                {

                    return View(product);

                }

                ViewBag.ErrorMessage = "The  product was not found.";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }

        }

        public async Task<ActionResult> AddProduct()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            Product product = new Product();
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProduct(Product product, HttpPostedFileBase selectedFile)
        {
            try
            {

                if (selectedFile != null)
                {
                    if (selectedFile != null)
                    {
                        string productImageName = await _fileUploadService.SaveProductImageAsync(selectedFile);
                        product.ProductImageName = productImageName;
                    }
                    product.ProductImageName = selectedFile.FileName;

                }

                await _repository.AddProductAsync(product);
                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the product.";
                ViewBag.ExceptionDetails = ex.Message;
                return View(product);
            }
        }

        public async Task<ActionResult> EditProduct(int ProductId)
        {
            try
            {
                var product = await _repository.GetProductByIdAsync(ProductId);

                TempData["ProductImage"] = product.ProductImage;
                TempData["ProductImageName"] = product.ProductImageName;

                var categories = await _categoryRepository.GetAllCategoriesAsync();
                ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);


                return View(product);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProduct(Product product, HttpPostedFileBase selectedFile)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {

                var categoryExists = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
                if (categoryExists == null)
                {
                    return View("CategoryError");
                }


                if (selectedFile != null)
                {
                    if (selectedFile != null)
                    {
                        string productImageName = await _fileUploadService.SaveProductImageAsync(selectedFile);
                        product.ProductImageName = productImageName;
                    }

                }
                else if (TempData["ProductImage"] != null && TempData["ProductImageName"] != null)
                {
                    product.ProductImage = (byte[])TempData["ProductImage"];
                    product.ProductImageName = (string)TempData["ProductImageName"];
                }

                await _repository.UpdateProductAsync(product);

                return RedirectToAction("DisplayProducts");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> DeleteProduct(int ProductId)
        {
            try
            {
                var product = await _repository.GetProductByIdAsync(ProductId);

                if (product == null)
                {
                    return HttpNotFound("Category not found.");
                }

                await _repository.DeleteProductAsync(ProductId);

                return RedirectToAction("DisplayProducts");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}