using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Services
{
    public interface IDealService
    {
        List<Deal> deals { get; set; }
        Task GetDeals();
        Task<Deal> GetDealById(int id);
        Task<int> AddDeal(Deal deal);
    }
}
