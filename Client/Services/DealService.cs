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
            try
            {
                // Load from server API
                var response = await _httpClient.GetAsync("api/deals");
                
                if (response.IsSuccessStatusCode)
                {
                    deals = await response.Content.ReadFromJsonAsync<List<WeeklyDeal>>() ?? new List<WeeklyDeal>();
                }
                else
                {
                    deals = new List<WeeklyDeal>();
                }
            }
            catch
            {
                // Fallback to empty list if API fails
                deals = new List<WeeklyDeal>();
            }
        }

        public async Task<WeeklyDeal> GetDealById(int id)
        {
            await GetDeals(); // Ensure deals are loaded
            var deal = deals.FirstOrDefault(d => d.Id == id);
            
            if (deal == null)
            {
                throw new Exception($"Deal with ID {id} not found");
            }
            
            return deal;
        }


        public async Task<int> AddDeal(WeeklyDeal deal)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/deals", deal);
                
                if (response.IsSuccessStatusCode)
                {
                    var createdDeal = await response.Content.ReadFromJsonAsync<WeeklyDeal>();
                    if (createdDeal != null)
                    {
                        // Refresh local cache
                        await GetDeals();
                        return createdDeal.Id;
                    }
                }
                
                throw new Exception("Failed to add deal");
            }
            catch
            {
                throw new Exception("Failed to add deal");
            }
        }

        public async Task<int> UpdateDeal(int id, WeeklyDeal deal)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/deals/{id}", deal);
                
                if (response.IsSuccessStatusCode)
                {
                    // Refresh local cache
                    await GetDeals();
                    return id;
                }
                
                throw new Exception("Failed to update deal");
            }
            catch
            {
                throw new Exception("Failed to update deal");
            }
        }

        public async Task<int> DeleteDeal(int id)
        {
            try
            {
                // The server expects a POST with method override header
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/deals/{id}");
                request.Headers.Add("X-HTTP-Method-Override", "DELETE");
                
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    // Refresh local cache
                    await GetDeals();
                    return id;
                }
                
                throw new Exception("Failed to delete deal");
            }
            catch
            {
                throw new Exception("Failed to delete deal");
            }
        }


    }
}
