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

        public Task<Deal> GetDealById(int id)
        {
            throw new NotImplementedException();
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


    }
}
