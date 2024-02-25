using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Services
{
    public interface IProductService
    {
        List<ProductItem> Products { get; set; }
        List<BusinessCategories> Categories { get; set; }
        Task LoadAllProducts();
        Task LoadAllCategories();
        Task<List<ProductItem>> GetProductsByCategory(int categoryId);
        Task<List<ProductItem>> SearchProducts(string searchTerm);
    }
}
