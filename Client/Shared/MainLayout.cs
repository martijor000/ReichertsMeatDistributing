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
        private bool IsMenuOpen { get; set; }

        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        private void CloseMenu()
        {
            IsMenuOpen = false;
        }

        private BusinessCategory SelectedCategory { get; set; } = BusinessCategory.All;


        private void SelectCategory(BusinessCategory category)
        {
            SelectedCategory = category;
            NavigationManager.NavigateTo($"/Products?category={category}", forceLoad: true);
        }

    }
}
