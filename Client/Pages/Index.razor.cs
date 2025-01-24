using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;
using System.Runtime.CompilerServices;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Index
    {
        [Inject]
        private IProductsService _productService { get; set; }
        private static List<ProductItem>? _products;
        public static bool IsLoaded => _products != null;

        protected override async Task OnInitializedAsync()
        {
            if (!Index.IsLoaded)
            {
                await _productService.GetProducts();
                var products = _productService._products;
                Index.SetProducts(products);
            }
        }

        public static void SetProducts(List<ProductItem> products)
        {
            _products = products;
        }

        public static List<ProductItem>? GetProducts()
        {
            return _products;
        }
    }
}

