using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Index
    {

        private List<Deal> deals;
        private Timer timer;
        private int totalSlides;
        private int currentSlideIndex;
        private Deal currentDeal;

        protected override async Task OnInitializedAsync()
        {
            await LoadDealsAsync();

            if (deals.Count != 0)
            {
                timer = new Timer(UpdateDeal, null, 0, 10000);
                currentSlideIndex = 0;
                currentDeal = deals[currentSlideIndex];
            }
        }

        private async Task LoadDealsAsync()
        {
            await ideal.GetDeals();
            deals = new List<Deal>(ideal.deals);
            if (deals.Count != 0)
            {
                totalSlides = deals.Count;
            }
        }


        private void UpdateDeal(object state)
        {
            if (currentSlideIndex < totalSlides - 1)
            {
                currentSlideIndex++;
            }
            else
            {
                currentSlideIndex = 0;
            }

            currentDeal = deals[currentSlideIndex];
            InvokeAsync(StateHasChanged);
        }

        private void PreviousSlideHandler()
        {
            if (currentSlideIndex + 1 > totalSlides - 1)
            {
                currentSlideIndex--;
            }
            else
            {
                currentSlideIndex = 0;
            }

            currentDeal = deals[currentSlideIndex];
        }

        private void NextSlideHandler()
        {
            if (currentSlideIndex < totalSlides - 1)
            {
                currentSlideIndex++;
            }
            else
            {
                currentSlideIndex = 0;
            }

            currentDeal = deals[currentSlideIndex];
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
