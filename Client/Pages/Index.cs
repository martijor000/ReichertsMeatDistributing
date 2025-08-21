using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Index
    {
        List<WeeklyDeal> deals;

        protected override async Task OnInitializedAsync()
        {
            await ideal.GetDeals();
            deals = ideal.deals;
        }
    }
}

