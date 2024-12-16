using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;


namespace OnlineStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly StoreDbContext _dbContext;
        public ProductController()
        {
            _dbContext = new StoreDbContext();
        }

        public  async Task<ActionResult> DisplayProducts(int page = 1 ,int pageSize = 10)
        {
           
            try
            {
                _dbContext.Configuration.LazyLoadingEnabled = false;

                var Page = (page - 1) * pageSize;

                var products = await _dbContext.Products
                                         .Include(p => p.Category)
                                         .Where(p => p.Discontinued == false)
                                         .OrderBy(p => p.ProductId)
                                         .Skip(Page)
                                         .Take(pageSize)
                                         .ToListAsync();

                var totalProducts = await _dbContext.Products
                                             .Where(p => p.Discontinued == false)
                                             .CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

                return View(products);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }
        }

        public async Task<ActionResult> DisplayProduct(int ProductId)
        {
            try
            {
                _dbContext.Configuration.LazyLoadingEnabled = false;

                Product product = await _dbContext.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.ProductId == ProductId && p.Discontinued == false)
                                 .FirstOrDefaultAsync();

                return View(product);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }

        }

        public  ActionResult AddProduct()
        {
            ViewBag.CategoryId =  new SelectList(_dbContext.Categories, "CategoryId", "CategoryName");
            Product product = new Product();
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProduct(Product product,HttpPostedFileBase selectedFile)
        {
            try
            {

                if (selectedFile != null)
                {
                    string DirectoryPath = Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(DirectoryPath))
                    {
                        Directory.CreateDirectory(DirectoryPath);
                    }

                    selectedFile.SaveAs(DirectoryPath + selectedFile.FileName);
                    BinaryReader br = new BinaryReader(selectedFile.InputStream);
                    product.ProductImage = br.ReadBytes(selectedFile.ContentLength);
                    product.ProductImageName = selectedFile.FileName;

                }

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("DisplayProducts");
            }
            catch(Exception ex)
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
                Product product = await _dbContext.Products.FindAsync(ProductId);

                TempData["ProductImage"] = product.ProductImage;
                TempData["ProductImageName"] = product.ProductImageName;

                ViewBag.CategoryId = new SelectList(_dbContext.Categories, "CategoryId", "CategoryName", product.CategoryId);
                return View(product);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProduct(Product product,HttpPostedFileBase selectedFile)
        {
            try
            {
                if (selectedFile != null)
                {
                    string DirectoryPath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(DirectoryPath))
                    {
                        Directory.CreateDirectory(DirectoryPath);
                    }
                    selectedFile.SaveAs(DirectoryPath + selectedFile.FileName);
                    BinaryReader br = new BinaryReader(selectedFile.InputStream);
                    product.ProductImage = br.ReadBytes(selectedFile.ContentLength);
                    product.ProductImageName = selectedFile.FileName;
                }
                else if (TempData["ProductImage"] != null && TempData["ProductImageName"] != null)
                {
                    product.ProductImage = (byte[])TempData["ProductImage"];
                    product.ProductImageName = (string)TempData["ProductImageName"];
                }

                _dbContext.Entry(product).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
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
                Product product = await _dbContext.Products.FindAsync(ProductId);

                if (product == null)
                {
                    return HttpNotFound("Category not found.");
                }

                product.Discontinued = true;
                _dbContext.Entry(product).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("DisplayProducts");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}