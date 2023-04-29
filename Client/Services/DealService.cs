using ReichertsMeatDistributing.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace ReichertsMeatDistributing.Client.Services
{
    public class DealService : IDealService
    {

        private readonly HttpClient _httpClient;
        public List<Deal> deals { get; set; } = new List<Deal>();
        public HttpClient? HttpClient { get; }


        public DealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetDeals()
        {
            var response = await _httpClient.GetAsync("api/Deals");
        
            if (response.IsSuccessStatusCode)
            {
                deals = await response.Content.ReadFromJsonAsync<List<Deal>>();
            }
            else
            {
                // handle error
            }
        }

        public async Task<Deal> GetDealById(int id)
        {
            var response = await _httpClient.GetAsync($"api/Deals/{id}");

            if (response.IsSuccessStatusCode)
            {
                var deal = await response.Content.ReadFromJsonAsync<Deal>();
                return deal;
            }
            else
            {
                throw new Exception($"Failed to retrieve deal. Status code: {response.StatusCode}");
            }
        }


        public async Task<int> AddDeal(Deal deal)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Deals", deal);

            if (response.IsSuccessStatusCode)
            {
                var createdDeal = await response.Content.ReadFromJsonAsync<Deal>();
                return createdDeal.Id;
            }
            else
            {
                throw new Exception($"Failed to add deal. Status code: {response.StatusCode}");
            }
        }

        public async Task<int> UpdateDeal(int id, Deal deal)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Deals/{id}", deal);

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
            var response = await _httpClient.DeleteAsync($"api/Deals/{id}");

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
