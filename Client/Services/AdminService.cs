using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Client.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;

        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAdminEmail()
        {
            // Make a request to your API endpoint to retrieve the admin email
            // Replace "api/admin/email" with the actual endpoint
            var response = await _httpClient.GetAsync("api/admin/email");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to get the admin email
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle the error or return a default value
                return "admin@example.com"; // Default email if the request fails
            }
        }
    }
}
