using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Products
    {
        public List<ProductItem> ProductItems { get; set; } 
        public List<ProductItem> DisplayedProductItems { get; set; }
        public BusinessCategory SelectedCategory { get; set; } 
        public int PageSize { get; set; } = 10;
        public int CurrentPageSize { get; set; } = 10;
        public bool ShowLoadMoreButton { get; set; } = true;
        public string Search { get; set; } = "";
        private ProductRepository ProductRepo = new ProductRepository();

        protected override void OnInitialized()
        {
            ProductItems = ProductRepo.GetAllProducts();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DisplayedProductItems = ProductItems;

            if (!string.IsNullOrEmpty(Search))
            {
                DisplayedProductItems = DisplayedProductItems
                    .Where(item => item.ItemDescription.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                                    item.ItemID.Contains(Search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                UpdateLoadMoreButtonVisibility();

            }
            else
            {
                UpdateLoadMoreButtonVisibility();
                UpdateDisplayedItems();
            }
        }

        public void LoadMore()
        {
            CurrentPageSize += PageSize;
            UpdateDisplayedItems();
            UpdateLoadMoreButtonVisibility();
        }

        private void SelectedCat()
        {
            switch (SelectedCategory)
            {
                case BusinessCategory.All:
                    ProductItems = ProductRepo.GetAllProducts();
                    ApplyFilters();
                    break;
                case BusinessCategory.Bars:
                    ProductItems = ProductRepo.GetBarProducts();
                    ApplyFilters();
                    break;
                case BusinessCategory.Restaurants:
                    ProductItems = ProductRepo.GetRestaurantsProducts();
                    ApplyFilters();
                    break;
                case BusinessCategory.BurgerBars:
                    ProductItems = ProductRepo.GetBurger_BarsProducts();
                    ApplyFilters();
                    break;
                case BusinessCategory.CoffeeShops:
                    ProductItems = ProductRepo.GetCoffee_ShopsProducts();
                    ApplyFilters();
                    break;
                case BusinessCategory.ConvenienceStores:
                    ProductItems = ProductRepo.GetConvenience_StoresProducts();
                    ApplyFilters();
                    break;
            }
        }

        private void UpdateDisplayedItems()
        {
            DisplayedProductItems = ProductItems
                .Take(CurrentPageSize)
                .ToList();
        }

        private void UpdateSearch(ChangeEventArgs e)
        {
            Search = e.Value.ToString();
            ApplyFilters();
        }

        public void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int size))
            {
                PageSize = size;
                CurrentPageSize = PageSize;
                ApplyFilters();
            }
        }

        private void UpdateLoadMoreButtonVisibility()
        {
            if (string.IsNullOrEmpty(Search))
            {
                ShowLoadMoreButton = CurrentPageSize < ProductItems.Count;
            }
            else
            {
                ShowLoadMoreButton = false;
            }
        }
    }
}
