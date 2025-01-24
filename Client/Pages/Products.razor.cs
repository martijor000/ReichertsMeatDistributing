using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Shared;
using Microsoft.JSInterop;

namespace ReichertsMeatDistributing.Client.Pages
{
    public partial class Products
    {
        private List<ProductItem>? productList;
        private List<ProductItem>? filteredProducts;
        private string searchQuery = string.Empty;

        [Inject]
        private IProductsService _productService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await JSRuntime.InvokeVoidAsync("window.scrollTo", 0, 0);
            if (Index.IsLoaded)
            {
                productList = Index.GetProducts();
            }
            else
            {
                await GetProducts();
            }
            FilterProducts();
        }

        private async Task GetProducts()
        {
            await _productService.GetProducts();
            productList = _productService._products;
            filteredProducts = productList;
        }

        private void HandleInput(ChangeEventArgs e)
        {
            searchQuery = e.Value.ToString();
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (productList != null)
            {
                filteredProducts = productList
                    .Where(p => p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }
    }
}





