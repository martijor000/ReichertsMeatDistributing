using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Services
{
    public class ProductService : IProductService
    {
        public List<ProductItem> Products { get; set; } = new List<ProductItem>();
        public List<BusinessCategories> Categories { get; set; } = new List<BusinessCategories>();

        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LoadAllProducts()
        {
            var response = await _httpClient.GetAsync("api/products");

            if (response.IsSuccessStatusCode)
            {
                Products = await response.Content.ReadFromJsonAsync<List<ProductItem>>();
            }
            else
            {
                // handle error
            }
        }

        public async Task LoadAllCategories()
        {
            var response = await _httpClient.GetAsync("api/categories");

            if (response.IsSuccessStatusCode)
            {
                Categories = await response.Content.ReadFromJsonAsync<List<BusinessCategories>>();
            }
            else
            {
                // handle error
            }
        }

        public async Task<List<ProductItem>> GetProductsByCategory(int categoryId)
        {
            var response = await _httpClient.GetAsync($"api/products/category/{categoryId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductItem>>();
            }
            else
            {
                // handle error
                return new List<ProductItem>();
            }
        }

        public async Task<List<ProductItem>> SearchProducts(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"api/products/search?term={searchTerm}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductItem>>();
            }
            else
            {
                // handle error
                return new List<ProductItem>();
            }
        }
    }
}
