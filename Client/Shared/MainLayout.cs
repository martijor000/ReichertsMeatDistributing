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

        [JSInvokable]
        public static void CloseMenuOnResize()
        {
            // This will be called from JavaScript when screen is resized to desktop
            // The menu will be hidden by CSS on desktop anyway
        }

        private BusinessCategory SelectedCategory { get; set; } = BusinessCategory.All;


        private void SelectCategory(BusinessCategory category)
        {
            SelectedCategory = category;
            NavigationManager.NavigateTo($"/Products?category={category}", forceLoad: true);
        }

    }
}
