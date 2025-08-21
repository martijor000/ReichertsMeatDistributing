using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Server.Data;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Services
{
    public class ProductSeederService
    {
        private readonly AppDbContext _context;

        public ProductSeederService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedProductsAsync()
        {
            try
            {
                // Check if Products table has the new columns by trying a query
                var existingCount = await _context.Products.CountAsync();
                
                // Only seed if there are no products
                if (existingCount > 0)
                {
                    Console.WriteLine($"Products already exist in database ({existingCount} products). Skipping seed.");
                    Console.WriteLine("Use ForceSeedProductsAsync() to clear and reseed the database.");
                    return; // Products already exist
                }

                var products = ProductSeedData.GetSeedProducts();
                Console.WriteLine($"Retrieved {products.Count} products from seed data source.");

                // Check for duplicates and log them
                var duplicateGroups = products.GroupBy(p => p.ItemID).Where(g => g.Count() > 1).ToList();
                if (duplicateGroups.Any())
                {
                    Console.WriteLine($"Found {duplicateGroups.Count} duplicate ItemIDs:");
                    foreach (var group in duplicateGroups.Take(5)) // Log first 5 duplicates
                    {
                        Console.WriteLine($"  ItemID '{group.Key}' appears {group.Count()} times");
                    }
                }

                // Remove duplicates based on ItemID and create new instances
                var uniqueProducts = products
                    .GroupBy(p => p.ItemID)
                    .Select(g => g.First())
                    .Where(p => !string.IsNullOrWhiteSpace(p.ItemID) && 
                                !string.IsNullOrWhiteSpace(p.StockingUM) && 
                                !string.IsNullOrWhiteSpace(p.ItemDescription))
                    .ToList();

                Console.WriteLine($"After filtering: {uniqueProducts.Count} unique products (filtered out {products.Count - uniqueProducts.Count} products)");
                
                // Log any products that were filtered out due to validation
                var filteredOut = products.Where(p => string.IsNullOrWhiteSpace(p.ItemID) || 
                                                     string.IsNullOrWhiteSpace(p.StockingUM) || 
                                                     string.IsNullOrWhiteSpace(p.ItemDescription)).ToList();
                if (filteredOut.Count > 0)
                {
                    Console.WriteLine($"Filtered out {filteredOut.Count} products due to invalid data:");
                    foreach (var product in filteredOut.Take(5)) // Log first 5 filtered products
                    {
                        Console.WriteLine($"  ItemID: '{product.ItemID}', UM: '{product.StockingUM}', Desc: '{product.ItemDescription}'");
                    }
                }

                Console.WriteLine($"Seeding {uniqueProducts.Count} unique products to database...");

                // Add all unique products from the repository
                foreach (var product in uniqueProducts)
                {
                    try
                    {
                        // Validate product data
                        if (string.IsNullOrWhiteSpace(product.ItemID))
                        {
                            Console.WriteLine($"Warning: Product with null/empty ItemID found, skipping...");
                            continue;
                        }

                        // Set default values for new fields
                        product.Category = DetermineCategory(product.ItemDescription, product.ItemID);
                        product.IsActive = true;
                        product.CreatedDate = DateTime.UtcNow;
                        
                        _context.Products.Add(product);
                    }
                    catch (Exception productEx)
                    {
                        Console.WriteLine($"Error processing product {product.ItemID}: {productEx.Message}");
                        throw; // Re-throw to stop the process
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Database save completed successfully.");
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Error saving to database: {saveEx.Message}");
                    Console.WriteLine($"Stack trace: {saveEx.StackTrace}");
                    throw;
                }

                Console.WriteLine($"Products seeded successfully! Total products in database: {await _context.Products.CountAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Don't throw - let the app continue running
            }
        }

        public async Task ForceSeedProductsAsync()
        {
            try
            {
                Console.WriteLine("Starting force seed process...");
                
                // Clear existing products first
                var existingProducts = await _context.Products.ToListAsync();
                if (existingProducts.Any())
                {
                    Console.WriteLine($"Found {existingProducts.Count} existing products. Clearing database...");
                    _context.Products.RemoveRange(existingProducts);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Cleared {existingProducts.Count} existing products from database.");
                }
                else
                {
                    Console.WriteLine("No existing products found in database.");
                }

                // Get the complete product list from seed data
                var products = ProductSeedData.GetSeedProducts();
                Console.WriteLine($"Retrieved {products.Count} products from seed data source.");
                
                // Log some sample products to verify data integrity
                if (products.Any())
                {
                    var sampleProduct = products.First();
                    Console.WriteLine($"Sample product - ID: '{sampleProduct.ItemID}', UM: '{sampleProduct.StockingUM}', Desc: '{sampleProduct.ItemDescription}'");
                }

                // Check for duplicates and log them
                var duplicateGroups = products.GroupBy(p => p.ItemID).Where(g => g.Count() > 1).ToList();
                if (duplicateGroups.Any())
                {
                    Console.WriteLine($"Found {duplicateGroups.Count} duplicate ItemIDs:");
                    foreach (var group in duplicateGroups.Take(5)) // Log first 5 duplicates
                    {
                        Console.WriteLine($"  ItemID '{group.Key}' appears {group.Count()} times");
                    }
                }

                // Remove duplicates based on ItemID and create new instances
                var uniqueProducts = products
                    .GroupBy(p => p.ItemID)
                    .Select(g => g.First())
                    .Where(p => !string.IsNullOrWhiteSpace(p.ItemID) && 
                                !string.IsNullOrWhiteSpace(p.StockingUM) && 
                                !string.IsNullOrWhiteSpace(p.ItemDescription))
                    .ToList();

                Console.WriteLine($"After filtering: {uniqueProducts.Count} unique products (filtered out {products.Count - uniqueProducts.Count} products)");
                
                // Log any products that were filtered out due to validation
                var filteredOut = products.Where(p => string.IsNullOrWhiteSpace(p.ItemID) || 
                                                     string.IsNullOrWhiteSpace(p.StockingUM) || 
                                                     string.IsNullOrWhiteSpace(p.ItemDescription)).ToList();
                if (filteredOut.Any())
                {
                    Console.WriteLine($"Filtered out {filteredOut.Count} products due to invalid data:");
                    foreach (var product in filteredOut.Take(5)) // Log first 5 filtered products
                    {
                        Console.WriteLine($"  ItemID: '{product.ItemID}', UM: '{product.StockingUM}', Desc: '{product.ItemDescription}'");
                    }
                }

                Console.WriteLine($"Force seeding {uniqueProducts.Count} unique products to database...");

                // Add all unique products from the repository
                foreach (var product in uniqueProducts)
                {
                    try
                    {
                        // Validate product data
                        if (string.IsNullOrWhiteSpace(product.ItemID))
                        {
                            Console.WriteLine($"Warning: Product with null/empty ItemID found, skipping...");
                            continue;
                        }

                        // Set default values for new fields
                        product.Category = DetermineCategory(product.ItemDescription, product.ItemID);
                        product.IsActive = true;
                        product.CreatedDate = DateTime.UtcNow;
                        
                        _context.Products.Add(product);
                    }
                    catch (Exception productEx)
                    {
                        Console.WriteLine($"Error processing product {product.ItemID}: {productEx.Message}");
                        throw; // Re-throw to stop the process
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Database save completed successfully.");
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Error saving to database: {saveEx.Message}");
                    Console.WriteLine($"Stack trace: {saveEx.StackTrace}");
                    throw;
                }

                var finalCount = await _context.Products.CountAsync();
                Console.WriteLine($"Products force seeded successfully! Total products in database: {finalCount}");
                
                if (finalCount != uniqueProducts.Count)
                {
                    Console.WriteLine($"Warning: Expected {uniqueProducts.Count} products but found {finalCount} in database.");
                }

                // Log category distribution
                var categoryDistribution = await _context.Products
                    .GroupBy(p => p.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync();
                
                Console.WriteLine("Category distribution:");
                foreach (var cat in categoryDistribution.OrderBy(c => c.Category))
                {
                    Console.WriteLine($"  {cat.Category}: {cat.Count} products");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error force seeding products: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw for debugging
            }
        }

        public int GetRepositoryProductCount()
        {
            try
            {
                var products = ProductSeedData.GetSeedProducts();
                var uniqueProducts = products
                    .GroupBy(p => p.ItemID)
                    .Select(g => g.First())
                    .Where(p => !string.IsNullOrWhiteSpace(p.ItemID) && 
                                !string.IsNullOrWhiteSpace(p.StockingUM) && 
                                !string.IsNullOrWhiteSpace(p.ItemDescription))
                    .ToList();
                
                Console.WriteLine($"Repository contains {products.Count} total products, {uniqueProducts.Count} unique valid products");
                return uniqueProducts.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting repository product count: {ex.Message}");
                return 0;
            }
        }

        public async Task<Dictionary<BusinessCategory, int>> GetDatabaseCategoryDistributionAsync()
        {
            try
            {
                var distribution = await _context.Products
                    .GroupBy(p => p.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Category, x => x.Count);
                
                return distribution;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting category distribution: {ex.Message}");
                return new Dictionary<BusinessCategory, int>();
            }
        }

        private BusinessCategory DetermineCategory(string description, string itemId)
        {
            var desc = description.ToLower();
            var id = itemId.ToLower();

            // Restaurant products (meat, proteins, ingredients)
            if (desc.Contains("beef") || desc.Contains("steak") || desc.Contains("ground beef") ||
                desc.Contains("chicken") || desc.Contains("breast") || desc.Contains("thigh") ||
                desc.Contains("pork") || desc.Contains("bacon") || desc.Contains("ham") ||
                desc.Contains("sausage") || desc.Contains("hot dog") || desc.Contains("burger") ||
                desc.Contains("meat") || desc.Contains("protein") || desc.Contains("cut") ||
                desc.Contains("loin") || desc.Contains("roast") || desc.Contains("tenderloin") ||
                desc.Contains("rib") || desc.Contains("chop") || desc.Contains("fillet") ||
                desc.Contains("deli") || desc.Contains("cold cut") || desc.Contains("lunch meat"))
            {
                return BusinessCategory.Restaurants;
            }
            // Bar products (alcoholic beverages)
            else if (desc.Contains("beer") || desc.Contains("wine") || desc.Contains("alcohol") ||
                     desc.Contains("whiskey") || desc.Contains("vodka") || desc.Contains("rum") ||
                     desc.Contains("gin") || desc.Contains("tequila") || desc.Contains("liquor") ||
                     desc.Contains("spirit") || desc.Contains("cocktail") || desc.Contains("mixer"))
            {
                return BusinessCategory.Bars;
            }
            // Burger Bar products (breads, buns, condiments)
            else if (desc.Contains("bread") || desc.Contains("roll") || desc.Contains("bun") || 
                     desc.Contains("brioche") || desc.Contains("croissant") || desc.Contains("bagel") ||
                     desc.Contains("ketchup") || desc.Contains("mustard") || desc.Contains("mayo") ||
                     desc.Contains("relish") || desc.Contains("pickle") || desc.Contains("onion") ||
                     desc.Contains("lettuce") || desc.Contains("tomato") || desc.Contains("cheese slice"))
            {
                return BusinessCategory.BurgerBars;
            }
            // Coffee Shop products
            else if (desc.Contains("coffee") || desc.Contains("tea") || desc.Contains("beverage") ||
                     desc.Contains("cream") || desc.Contains("sugar") || desc.Contains("syrup") ||
                     desc.Contains("milk") || desc.Contains("non-dairy") || desc.Contains("espresso"))
            {
                return BusinessCategory.CoffeeShops;
            }
            // Convenience Store products (everything else)
            else
            {
                return BusinessCategory.ConvenienceStores;
            }
        }
    }
}
