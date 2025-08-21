using Microsoft.AspNetCore.Components;
using ReichertsMeatDistributing.Shared;
using ReichertsMeatDistributing.Client.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Web;

namespace ReichertsMeatDistributing.Client.Pages
{
    public partial class Login
    {
        private string errorMessage = "";
        private bool isLoading = false;

        [Inject]
        private HttpClient HttpClient { get; set; } = null!;

        protected override void OnInitialized()
        {
            try
            {
                // Check for error parameter in URL
                var uri = new Uri(NavigationManager.Uri);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var error = query["error"];
                
                if (error == "authentication_failed")
                {
                    errorMessage = "Authentication failed. Please try again.";
                }
                else if (error == "unauthorized")
                {
                    errorMessage = "You are not authorized to access this application. Only authorized email addresses can access the admin panel.";
                }
                else if (error == "no_email")
                {
                    errorMessage = "No email address found in your Google account. Please ensure your Google account has an email address.";
                }
                else if (error == "google_error")
                {
                    errorMessage = "Google authentication encountered an error. This is usually temporary. Please try again in a few moments.";
                }
            }
            catch (Exception ex)
            {
                // Handle any initialization errors silently
                Console.WriteLine($"Login initialization error: {ex.Message}");
            }
        }

        private async Task SignInWithGoogle()
        {
            try
            {
                isLoading = true;
                errorMessage = "";
                StateHasChanged();

                // Redirect to Google OAuth endpoint
                var googleLoginUrl = $"{NavigationManager.BaseUri}api/admin/google-login";
                await JSRuntime.InvokeVoidAsync("open", googleLoginUrl, "_self");
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred during sign-in. Please try again.";
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}
