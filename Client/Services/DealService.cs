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
                // For GitHub Pages, load from static JSON file
                deals = await _httpClient.GetFromJsonAsync<List<WeeklyDeal>>("deals.json");
                
                if (deals == null)
                {
                    deals = new List<WeeklyDeal>();
                }
            }
            catch
            {
                // Fallback to empty list if file not found
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
            // For static hosting, admin features need to be updated
            // For now, just add to local list
            await GetDeals();
            deal.Id = deals.Count > 0 ? deals.Max(d => d.Id) + 1 : 1;
            deals.Add(deal);
            return deal.Id;
        }

        public async Task<int> UpdateDeal(int id, WeeklyDeal deal)
        {
            // For static hosting, admin features need to be updated
            // For now, just update local list
            await GetDeals();
            var existingDeal = deals.FirstOrDefault(d => d.Id == id);
            if (existingDeal != null)
            {
                existingDeal.Name = deal.Name;
                existingDeal.Description = deal.Description;
                existingDeal.Price = deal.Price;
            }
            return id;
        }

        public async Task<int> DeleteDeal(int id)
        {
            // For static hosting, admin features need to be updated
            // For now, just remove from local list
            await GetDeals();
            var dealToRemove = deals.FirstOrDefault(d => d.Id == id);
            if (dealToRemove != null)
            {
                deals.Remove(dealToRemove);
            }
            return id;
        }


    }
}
