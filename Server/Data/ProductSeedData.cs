using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Data
{
    public static class ProductSeedData
    {
        public static List<ProductItem> GetSeedProducts()
        {
            // Use the full product list from the repository
            var repository = new ProductRepository();
            return repository.GetAllProducts();
        }
    }
}
