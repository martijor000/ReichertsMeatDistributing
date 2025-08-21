//using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReichertsMeatDistributing.Client;
using ReichertsMeatDistributing.Client.Pages;
using ReichertsMeatDistributing.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Simple client - server handles authentication

// Add other services
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IProductService, ProductService>();

await builder.Build().RunAsync();
