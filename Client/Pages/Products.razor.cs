using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ReichertsMeatDistributing.Shared;
using ReichertsMeatDistributing.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await GetProducts();
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





