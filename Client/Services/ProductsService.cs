using ReichertsMeatDistributing.Shared;
using System.Net.Http.Json;

public interface IProductsService
{
    Task GetProducts();
    List<ProductItem> _products { get; set; }
}

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    public List<ProductItem> _products { get; set; } = new List<ProductItem>();

    public ProductsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task GetProducts()
    {
        var response = await _httpClient.GetAsync("api/products");

        if (response.IsSuccessStatusCode)
        {
            _products = await response.Content.ReadFromJsonAsync<List<ProductItem>>();
        }
        else
        {
            // handle error
        }
    }
}
