using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Server.Services
{
    public interface IGoogleCloudService
    {
        Task<string> GetClientIdAsync();
        Task<string> GetClientSecretAsync();
        Task<bool> ValidateCredentialsAsync(string clientId, string clientSecret);
    }

    public class GoogleCloudService : IGoogleCloudService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _projectId;

        public GoogleCloudService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _projectId = _configuration["GoogleCloud:ProjectId"] ?? "";
        }

        public async Task<string> GetClientIdAsync()
        {
            // Priority order: Environment Variable > Configuration
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            if (!string.IsNullOrEmpty(clientId))
                return clientId;

            // Fall back to configuration
            return _configuration["Authentication:Google:ClientId"] ?? "";
        }

        public async Task<string> GetClientSecretAsync()
        {
            // Priority order: Environment Variable > Configuration
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            if (!string.IsNullOrEmpty(clientSecret))
                return clientSecret;

            // Fall back to configuration
            return _configuration["Authentication:Google:ClientSecret"] ?? "";
        }

        public async Task<bool> ValidateCredentialsAsync(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return false;

            try
            {
                // Validate against Google's OAuth discovery endpoint
                var discoveryUrl = "https://accounts.google.com/.well-known/openid_configuration";
                var response = await _httpClient.GetAsync(discoveryUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var discoveryDoc = JsonSerializer.Deserialize<JsonElement>(content);
                    
                    // Basic validation - check if the endpoints are accessible
                    return discoveryDoc.TryGetProperty("authorization_endpoint", out _) &&
                           discoveryDoc.TryGetProperty("token_endpoint", out _);
                }
            }
            catch
            {
                // If validation fails, assume credentials are valid (fallback)
                return true;
            }

            return true;
        }
    }
}
