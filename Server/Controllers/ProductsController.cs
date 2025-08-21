using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Server.Data;
using ReichertsMeatDistributing.Server.Services;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProductSeederService _seederService;

        public ProductsController(AppDbContext context, ProductSeederService seederService)
        {
            _context = context;
            _seederService = seederService;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<List<ProductItem>>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.ItemDescription)
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving products: {ex.Message}");
            }
        }

        // GET api/products/debug
        [HttpGet("debug")]
        public async Task<ActionResult> GetProductsDebug()
        {
            try
            {
                var totalCount = await _context.Products.CountAsync();
                var activeCount = await _context.Products.CountAsync(p => p.IsActive);
                var sampleProducts = await _context.Products.Take(5).ToListAsync();
                
                return Ok(new { 
                    TotalProducts = totalCount,
                    ActiveProducts = activeCount,
                    SampleProducts = sampleProducts,
                    Message = $"Database has {totalCount} total products, {activeCount} active"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error debugging products: {ex.Message}");
            }
        }

        // POST api/products/seed
        [HttpPost("seed")]
        public async Task<ActionResult> SeedProducts()
        {
            try
            {
                await _seederService.SeedProductsAsync();
                var count = await _context.Products.CountAsync();
                return Ok(new { Message = $"Seeding completed. Total products: {count}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error seeding products: {ex.Message}");
            }
        }

        // POST api/products/force-seed
        [HttpPost("force-seed")]
        public async Task<ActionResult> ForceSeedProducts()
        {
            try
            {
                await _seederService.ForceSeedProductsAsync();
                var count = await _context.Products.CountAsync();
                return Ok(new { Message = $"Force seeding completed. Total products: {count}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error force seeding products: {ex.Message}. Stack: {ex.StackTrace}");
            }
        }

        // POST api/products/direct-seed
        [HttpPost("direct-seed")]
        public async Task<ActionResult> DirectSeedProducts()
        {
            try
            {
                // Clear existing products
                var existingProducts = await _context.Products.ToListAsync();
                if (existingProducts.Any())
                {
                    _context.Products.RemoveRange(existingProducts);
                    await _context.SaveChangesAsync();
                }

                // Get seed data directly
                var seedProducts = ProductSeedData.GetSeedProducts();
                
                foreach (var product in seedProducts)
                {
                    // Set default values
                    product.Category = DetermineCategory(product.ItemDescription, product.ItemID);
                    product.IsActive = true;
                    product.CreatedDate = DateTime.UtcNow;
                    product.ModifiedDate = null;
                    
                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
                var count = await _context.Products.CountAsync();
                
                return Ok(new { 
                    Message = $"Direct seeding completed. Total products: {count}",
                    SeedProductsCount = seedProducts.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error direct seeding products: {ex.Message}. Stack: {ex.StackTrace}");
            }
        }

        private BusinessCategory DetermineCategory(string description, string itemId)
        {
            var desc = description.ToLower();
            var id = itemId.ToLower();

            if (desc.Contains("bread") || desc.Contains("roll") || desc.Contains("bun") || 
                id.Contains("baker") || desc.Contains("croiss"))
            {
                return BusinessCategory.BurgerBars;
            }
            else if (desc.Contains("coffee") || desc.Contains("tea") || desc.Contains("beverage"))
            {
                return BusinessCategory.CoffeeShops;
            }
            else if (desc.Contains("beer") || desc.Contains("wine") || desc.Contains("alcohol") ||
                     desc.Contains("whiskey") || desc.Contains("vodka"))
            {
                return BusinessCategory.Bars;
            }
            else if (desc.Contains("sauce") || desc.Contains("seasoning") || desc.Contains("spice") ||
                     desc.Contains("meat") || desc.Contains("beef") || desc.Contains("chicken") ||
                     desc.Contains("pork") || desc.Contains("cheese"))
            {
                return BusinessCategory.Restaurants;
            }
            else
            {
                return BusinessCategory.ConvenienceStores;
            }
        }

        // GET api/products/by-category/{category}
        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<List<ProductItem>>> GetProductsByCategory(BusinessCategory category)
        {
            try
            {
                var query = _context.Products.Where(p => p.IsActive);
                
                if (category != BusinessCategory.All)
                {
                    query = query.Where(p => p.Category == category);
                }

                var products = await query
                    .OrderBy(p => p.ItemDescription)
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving products: {ex.Message}");
            }
        }

        // GET api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductItem>> GetProduct(string id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                
                if (product == null || !product.IsActive)
                {
                    return NotFound();
                }
                
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving product: {ex.Message}");
            }
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<ProductItem>> CreateProduct(ProductItem product)
        {
            try
            {
                // Check if product ID already exists
                if (await _context.Products.AnyAsync(p => p.ItemID == product.ItemID))
                {
                    return Conflict("Product with this ID already exists");
                }

                product.CreatedDate = DateTime.UtcNow;
                product.ModifiedDate = null;
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetProduct), new { id = product.ItemID }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating product: {ex.Message}");
            }
        }

        // PUT api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, ProductItem product)
        {
            try
            {
                if (id != product.ItemID)
                {
                    return BadRequest("Product ID mismatch");
                }

                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Update properties
                existingProduct.ItemDescription = product.ItemDescription;
                existingProduct.StockingUM = product.StockingUM;
                existingProduct.Category = product.Category;
                existingProduct.Price = product.Price;
                existingProduct.IsActive = product.IsActive;
                existingProduct.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating product: {ex.Message}");
            }
        }

        // DELETE api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Soft delete - mark as inactive instead of removing
                product.IsActive = false;
                product.ModifiedDate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting product: {ex.Message}");
            }
        }

        // GET api/products/download-catalog
        [HttpGet("download-catalog")]
        public async Task<IActionResult> DownloadCatalog()
        {
            try
            {
                // In Blazor WebAssembly, the Client's wwwroot files are served by the Server
                // The path should be relative to the Server's static files
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Client", "wwwroot", "PDF", "Reicherts_Product_List.pdf");
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Product catalog PDF not found");
                }
                
                var fileName = "Reicherts_Product_Catalog.pdf";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error downloading catalog: {ex.Message}");
            }
        }
    }
}
