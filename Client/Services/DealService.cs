using ReichertsMeatDistributing.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace ReichertsMeatDistributing.Client.Services
{
    public class DealService : IDealService
    {

        private readonly HttpClient _httpClient;
        public List<WeeklyDeal> deals { get; set; } = new List<WeeklyDeal>();
        public List<ProductItem> products { get; set; } = new List<ProductItem>();
        public HttpClient? HttpClient { get; }


        public DealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async Task GetDeals()
        {
            var response = await _httpClient.GetAsync("api/deals");

            if (response.IsSuccessStatusCode)
            {
                deals = await response.Content.ReadFromJsonAsync<List<WeeklyDeal>>();
            }
            else
            {
                // handle error
            }
        }

        public async Task<WeeklyDeal> GetDealById(int id)
        {
            var response = await _httpClient.GetAsync($"api/deals/{id}");

            if (response.IsSuccessStatusCode)
            {
                var deal = await response.Content.ReadFromJsonAsync<WeeklyDeal>();
                return deal;
            }
            else
            {
                throw new Exception($"Failed to retrieve deal. Status code: {response.StatusCode}");
            }
        }


        public async Task<int> AddDeal(WeeklyDeal deal)
        {
            var response = await _httpClient.PostAsJsonAsync("api/deals", deal);

            if (response.IsSuccessStatusCode)
            {
                var createdDeal = await response.Content.ReadFromJsonAsync<WeeklyDeal>();
                return createdDeal.Id;
            }
            else
            {
                throw new Exception($"Failed to add deal. Status code: {response.StatusCode}");
            }
        }

        public async Task<int> UpdateDeal(int id, WeeklyDeal deal)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/deals/{id}", deal);

            if (response.IsSuccessStatusCode)
            {
                return id;
            }
            else
            {
                throw new Exception($"Failed to update deal. Status code: {response.StatusCode}");
            }
        }

        public async Task<int> DeleteDeal(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/deals/{id}");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return id;
            }
            else
            {
                throw new Exception($"Failed to delete deal. Status code: {response.StatusCode}");
            }
        }


    }
}
