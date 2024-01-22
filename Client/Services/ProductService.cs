using ReichertsMeatDistributing.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace ReichertsMeatDistributing.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public List<ProductItem> products { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<BusinessCategory> categories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HttpClient? HttpClient { get; }

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public Task GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public async Task GetAllProducts()
        {
            var response = await _httpClient.GetAsync("api/products");

            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadFromJsonAsync<List<ProductItem>>();
            }
            else
            {
                // handle error
            }
        }
    }
}
