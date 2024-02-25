using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Products
    {
        private List<ProductItem> DisplayedProductItems { get; set; } = new List<ProductItem>();
        private List<BusinessCategories> Categories { get; set; } = new List<BusinessCategories>(); 
        private BusinessCategories SelectedCategory { get; set; } = null; 
        private int PageSize { get; set; } = 10;
        private int CurrentPageSize { get; set; } = 10;
        private bool ShowLoadMoreButton { get; set; } = true;
        private string Search { get; set; } = "";

        [Inject]
        private ProductService ProductService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Load products and categories
            await LoadProducts();
            await LoadCategories();

            // Read the category query parameter from the URL
            //var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            //var queryParams = QueryHelpers.ParseQuery(uri.Query);
            //if (queryParams.TryGetValue("category", out var category))
            //{
            //    SelectedCategory = category;
            //}
            //else
            //{
            //    SelectedCategory = null;
            //}
        }


        private async Task LoadProducts()
        {
            await ProductService.LoadAllProducts();
            DisplayedProductItems = ProductService.Products;
            ApplyFilters();
        }

        private async Task LoadCategories()
        {
            await ProductService.LoadAllCategories();
            Categories = ProductService.Categories;
        }

        private void ApplyFilters()
        {
            var filteredItems = DisplayedProductItems.ToList();

            if (!string.IsNullOrEmpty(Search))
            {
                filteredItems = filteredItems
                    .Where(item => item.ItemDescription.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                                    item.ItemID.Contains(Search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (SelectedCategory != null) // Check if a category is selected
            {
                if (SelectedCategory.Category != "All")
                {
                    filteredItems = filteredItems
                        .Where(item => item.BusinessCategory == SelectedCategory)
                        .ToList();
                }
            }

            UpdateLoadMoreButtonVisibility(filteredItems);
            UpdateDisplayedItems(filteredItems);
        }

        public void LoadMore()
        {
            CurrentPageSize += PageSize;
            ApplyFilters();
        }

        private void SelectedCat(ChangeEventArgs e)
        {
            // Parse selected category dynamically
            var categoryName = e.Value.ToString();
            SelectedCategory = Categories.FirstOrDefault(c => c.Category == categoryName);
            ApplyFilters();
        }

        private void UpdateDisplayedItems(List<ProductItem> items)
        {
            DisplayedProductItems = items.Take(CurrentPageSize).ToList();
        }

        private void UpdateSearch(ChangeEventArgs e)
        {
            Search = e.Value.ToString();
            ApplyFilters();
        }

        private void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int size))
            {
                PageSize = size;
                CurrentPageSize = PageSize;
                ApplyFilters();
            }
        }

        private void UpdateLoadMoreButtonVisibility(List<ProductItem> items)
        {
            ShowLoadMoreButton = CurrentPageSize < items.Count;
        }
    }
}
