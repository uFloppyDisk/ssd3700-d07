using EmailDemo.Data;
using EmailDemo.Models;
using EmailDemo.ViewModels;

namespace EmailDemo.Repositories
{
    /// <summary>
    /// Repository for handling CRUD operations on the Product entity.
    /// </summary>
    public class ProductRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductRepo> _logger;

        /// <summary>
        /// Constructor to inject the database context and logger.
        /// </summary>
        /// <param name="db">Application database context.</param>
        /// <param name="logger">Logger for tracking errors and activity.</param>
        public ProductRepo(ApplicationDbContext db, ILogger<ProductRepo> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>List of products.</returns>
        public IEnumerable<ProductVM> GetAllProducts()
        {
            return _db.Products.Select(p => new ProductVM
            {
                ProductId = p.PkProductId,
                Description = p.Description,
                Price = p.Price
            }).OrderBy(p => p.ProductId);
        }

        /// <summary>
        /// Retrieves a single product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The matching product or null if not found.</returns>
        public ProductVM? GetProductById(int productId)
        {
            var product = _db.Products.Find(productId);
            if (product == null) return null;

            return new ProductVM
            {
                ProductId = product.PkProductId,
                Description = product.Description,
                Price = product.Price
            };
        }

        /// <summary>
        /// Creates a new product entry in the database.
        /// </summary>
        /// <param name="productVM">The product entity to create.</param>
        public string CreateProduct(ProductVM productVM)
        {
            string message = string.Empty;

            try
            {
                var product = new Product
                {
                    Description = productVM.Description ?? "",
                    Price = productVM.Price
                };

                _db.Products.Add(product);
                _db.SaveChanges();

                message = $"success|Product created successfully: {product.Description}";
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                message = $"error|Failed to create product: {ex.Message}";
                _logger.LogError(message);
            }
            return message;
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="productVM">The product entity with updated details.</param>
        public string UpdateProduct(ProductVM productVM)
        {
            string message = string.Empty;

            try
            {
                var product = _db.Products.Find(productVM.ProductId);
                if (product == null)
                {
                    message = $"warning|Product with ID: {productVM.ProductId} not found.";
                    _logger.LogWarning(message);
                }
                else
                {
                    product.Description = productVM.Description;
                    product.Price = productVM.Price;

                    _db.Products.Update(product);
                    _db.SaveChanges();

                    message = $"success|Product {product.PkProductId} updated successfully.";
                    _logger.LogInformation(message);
                }
            }
            catch (Exception ex)
            {
                message = $"error|Failed to update product with ID: {productVM.ProductId} : {ex.Message}";
                _logger.LogInformation(message);
            }

            return message;
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        public string DeleteProduct(int productId)
        {
            string message = string.Empty;

            try
            {
                var product = _db.Products.Find(productId);
                if (product == null)
                {
                    message = $"warning|Product with ID: {productId} not found.";
                    _logger.LogWarning(message);
                }
                else
                {
                    _db.Products.Remove(product);
                    _db.SaveChanges();

                    message = $"success|Product {productId} deleted successfully.";
                    _logger.LogInformation(message);
                }
            }
            catch (Exception ex)
            {
                message = $"error|Failed to delete product with ID: {productId} : {ex.Message}";
                _logger.LogInformation(message);
            }

            return message;
        }

        /// <summary>
        /// Uploads an image and associates it with a product.
        /// </summary>
        /// <param name="productId">Product ID.</param>
        /// <param name="imageFile">Uploaded image file.</param>
        /// <returns>Status message.</returns>
        public string AddProductImage(int productId, IFormFile imageFile)
        {
            string message = string.Empty;

            try
            {
                var product = _db.Products.Find(productId);
                if (product == null)
                {
                    return $"error|Product with ID: {productId} not found.";
                }

                // Check if the product already has an image
                var existingImage = _db.ProductImages.FirstOrDefault(pi => pi.FkProductId == productId);
                if (existingImage != null)
                {
                    _db.ProductImages.Remove(existingImage);
                    _db.SaveChanges();
                }

                using var stream = new MemoryStream();
                imageFile.CopyTo(stream);
                var imageBytes = stream.ToArray();

                var productImage = new ProductImage
                {
                    FkProductId = productId,
                    FileName = imageFile.FileName,
                    ContentType = imageFile.ContentType,
                    ImageData = imageBytes
                };

                _db.ProductImages.Add(productImage);
                _db.SaveChanges();

                message = "success|Image uploaded successfully!";
            }
            catch (Exception ex)
            {
                message = $"error|Failed to upload image: {ex.Message}";
            }

            return message;
        }

        public ProductImage? GetProductImageByProductId(int productId)
        {
            return _db.ProductImages
                      .Where(pi => pi.FkProductId == productId)
                      .OrderBy(pi => pi.PkProductImageId)
                      .FirstOrDefault();
        }


        public ProductImage? GetImagePlaceholder()
        {
            return _db.ProductImages.FirstOrDefault(pi => pi.FkProductId == 1);
        }
    }
}
