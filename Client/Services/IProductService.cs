using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Services
{
    public interface IProductService
    {
        List<ProductItem> products { get; set; }
        Task GetAllProducts();
        List<BusinessCategory> categories { get; set; }
        Task GetAllCategories();

    }
}
