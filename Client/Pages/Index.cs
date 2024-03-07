using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;
using System.Runtime.CompilerServices;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Index
    {
        List<WeeklyDeal> deals;

        string carouselName = "myCarousel";

        protected override async Task OnInitializedAsync()
        {
            await ideal.GetDeals(); // Assuming you have a method to fetch deals
            deals = ideal.deals;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("startCarousel", carouselName);
            }
        }
    }
}

