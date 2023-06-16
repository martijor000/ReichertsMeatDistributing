using Microsoft.AspNetCore.Components;
using ReichertsClassLib;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Client.Pages
{
    public partial class Login
    {
        private LoginModel loginModel = new LoginModel();
        private bool isLoginFailed = false;

        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private async Task SubmitLoginForm()
        {
            isLoginFailed = false;

            // Call the login API endpoint and pass the loginModel
            var response = await HttpClient.PostAsJsonAsync("api/Admin/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                AuthStateProvider.MarkUserAsAuthenticated("admin");
                NavigationManager.NavigateTo("/admin/deals");
            }
            else
            {
                // Failed login
                isLoginFailed = true;
            }
        }
    }
}
