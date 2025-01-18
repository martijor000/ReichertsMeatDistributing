using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ReichertsMeatDistributing.Shared;
using System.Drawing;

namespace ReichertsMeatDistributing.Client.Shared
{
    partial class MainLayout
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private BusinessCategory SelectedCategory { get; set; } = BusinessCategory.All;


        private void SelectCategory(BusinessCategory category)
        {
            SelectedCategory = category;
            NavigationManager.NavigateTo($"/ProductsService?category={category}", forceLoad: true);
        }

    }
}
