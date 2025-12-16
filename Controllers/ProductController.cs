using EmailDemo.Repositories;
using EmailDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailDemo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepo _productRepo;

        public ProductController(ProductRepo productRepo)
        {
            _productRepo = productRepo;

        }

        public IActionResult Index(string message = "")
        {
            ViewBag.Message = message;

            return View(_productRepo.GetAllProducts());
        }

        [Authorize]
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult Create(ProductVM product)
        {
            if (ModelState.IsValid)
            {
                string message = _productRepo.CreateProduct(product);
                return RedirectToAction(nameof(Index), new { message });
            }

            return View(product);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult DeleteProduct(int productId)
        {
            string message = _productRepo.DeleteProduct(productId);
            return RedirectToAction(nameof(Index), new { message });
        }

        [Authorize(Roles = "Manager")]
        public IActionResult UploadProductImage(int productId)
        {
            // Load the product and return the upload view
            ProductVM? product = _productRepo.GetProductById(productId);

            if (product == null)
            {
                return RedirectToAction(nameof(Index), new { message = $"Product with id: {productId} not found!" });
            }

            return View(product);
        }

        /// <summary>
        /// Uploads an image and associates it with a product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="imageFile">Uploaded image file.</param>
        /// <returns>Status message.</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult UploadProductImage(int productId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag["Message"] = "error|No image selected. Please choose an image.";
                return RedirectToAction(nameof(UploadProductImage), new { productId });
            }

            string message = _productRepo.AddProductImage(productId, imageFile);
            return RedirectToAction(nameof(Index), new { message });
        }

        public IActionResult GetProductImage(int productId)
        {
            var productImage = _productRepo.GetProductImageByProductId(productId);

            if (productImage != null && productImage.ImageData != null)
            {
                return File(productImage.ImageData, productImage.ContentType);
            }

            // Return placeholder image if no product image exists
            var placeholderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "placeholder.png");
            var imageBytes = System.IO.File.ReadAllBytes(placeholderPath);
            return File(imageBytes, "image/png");
        }
    }
}
