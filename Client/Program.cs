//using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReichertsMeatDistributing.Client;
using ReichertsMeatDistributing.Client.Pages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped<IDealService, DealService>(); --Maybe this is the issue that it's in client side not server side?

await builder.Build().RunAsync();
