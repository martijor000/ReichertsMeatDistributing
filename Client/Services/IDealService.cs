using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Services
{
    public interface IDealService
    {
        List<WeeklyDeal> deals { get; set; }
        Task GetDeals();
        Task<WeeklyDeal> GetDealById(int id);
        Task<int> AddDeal(WeeklyDeal deal);
        Task<int> UpdateDeal(int id, WeeklyDeal deal);
        Task<int> DeleteDeal(int id);
    }
}
