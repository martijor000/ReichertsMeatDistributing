using ReichertsMeatDistributing.Shared;
using System.Net.Http.Json;

namespace ReichertsMeatDistributing.Client.Services
{
    public interface IProductService
    {
        Task<List<ProductItem>> GetProducts();
        Task<List<ProductItem>> GetProductsByCategory(BusinessCategory category);
        Task<ProductItem?> GetProduct(string id);
        Task<ProductItem> AddProduct(ProductItem product);
        Task UpdateProduct(ProductItem product);
        Task DeleteProduct(string id);
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private List<ProductItem> _products = new();

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductItem>> GetProducts()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");
                if (response.IsSuccessStatusCode)
                {
                    _products = await response.Content.ReadFromJsonAsync<List<ProductItem>>() ?? new List<ProductItem>();
                    return _products;
                }
                return new List<ProductItem>();
            }
            catch
            {
                return new List<ProductItem>();
            }
        }

        public async Task<List<ProductItem>> GetProductsByCategory(BusinessCategory category)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/by-category/{category}");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ProductItem>>() ?? new List<ProductItem>();
                    return products;
                }
                return new List<ProductItem>();
            }
            catch
            {
                return new List<ProductItem>();
            }
        }

        public async Task<ProductItem?> GetProduct(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductItem>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProductItem> AddProduct(ProductItem product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", product);
                if (response.IsSuccessStatusCode)
                {
                    var createdProduct = await response.Content.ReadFromJsonAsync<ProductItem>();
                    if (createdProduct != null)
                    {
                        await GetProducts(); // Refresh local cache
                        return createdProduct;
                    }
                }
                throw new Exception("Failed to add product");
            }
            catch
            {
                throw new Exception("Failed to add product");
            }
        }

        public async Task UpdateProduct(ProductItem product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/products/{product.ItemID}", product);
                if (response.IsSuccessStatusCode)
                {
                    await GetProducts(); // Refresh local cache
                }
                else
                {
                    throw new Exception("Failed to update product");
                }
            }
            catch
            {
                throw new Exception("Failed to update product");
            }
        }

        public async Task DeleteProduct(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    await GetProducts(); // Refresh local cache
                }
                else
                {
                    throw new Exception("Failed to delete product");
                }
            }
            catch
            {
                throw new Exception("Failed to delete product");
            }
        }
    }
}
