using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Shared;
using System.Net.Http.Json;
using ReichertsMeatDistributing.Client.Services;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Client.Pages
{
    public partial class Products
    {
        private List<ProductItem>? productList;

        [Inject]
        private IProductsService _productService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetProducts();
            productList = _productService._products;
        }

        private async Task GetProducts()
        {
            await _productService.GetProducts();

        }
    }
}
