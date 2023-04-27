using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Pages
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
            LoadDealsAsync();

            if(deals != null)
            {
                timer = new Timer(UpdateDeal, null, 0, 10000);
                currentSlideIndex = 0;
                currentDeal = deals[currentSlideIndex];
            }
        }

        private void LoadDealsAsync()
        {
            deals = db.deals.ToList();

            if (deals != null)
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


