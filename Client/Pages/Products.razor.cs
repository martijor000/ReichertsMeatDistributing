using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Products
    {
        public List<ProductItem> ProductItems { get; set; }
        public List<ProductItem> FilteredProductItems { get; set; }
        public int pageSize { get; set; } = 10;
        public bool ShowNextButton { get; set; } = true;
        public int Page { get; set; } = 1;
        public string search = "";

        protected override void OnInitialized()
        {
            var productRepository = new ProductRepository();
            ProductItems = productRepository.GetCoffee_ShopsProducts();

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            FilteredProductItems = ProductItems;

            if (!string.IsNullOrEmpty(search))
            {
                FilteredProductItems = FilteredProductItems
                    .Where(item => item.ItemDescription.Contains(search, StringComparison.OrdinalIgnoreCase) || item.ItemID.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                LoadPage();
            }

            Page = 1;
        }

        public void LoadNextPage()
        {
            Page++;
            LoadPage();

            if (Page > 1 && FilteredProductItems.Count < pageSize)
            {
                // Reached the end, adjust the page number
                Page = (ProductItems.Count - 1) / pageSize + 1;
                LoadPage();
            }
        }


        public void LoadPreviousPage()
        {
            Page--;
            LoadPage();
        }

        public void LoadPage()
        {
            var startIndex = (Page - 1) * pageSize;
            FilteredProductItems = ProductItems
                .Skip(startIndex)
                .Take(pageSize)
                .ToList();

            if (startIndex + FilteredProductItems.Count >= ProductItems.Count)
            {
                // Reached the end, adjust the page number
                Page = (ProductItems.Count - 1) / pageSize + 1;
            }

            UpdateNextButtonVisibility();
        }

        public void OnPageSizeChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int size))
            {
                pageSize = size;
                ApplyFilters();
                UpdateNextButtonVisibility();
            }
        }
        private void UpdateNextButtonVisibility()
        {
            ShowNextButton = Page < (ProductItems.Count - 1) / pageSize + 1;
        }


    }
}
