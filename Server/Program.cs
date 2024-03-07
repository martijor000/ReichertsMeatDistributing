using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddScoped<IDealService, DealService>();
// Configure HttpClient with custom certificate validation
builder.Services.AddHttpClient<CustomHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://your-production-api-url/");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new CustomHttpClientHandler();

    // Access the certificate and validate it
    var certificatePath = "Certificate\\reichertsdistributing.der";
    var certificateBytes = System.IO.File.ReadAllBytes(certificatePath);

    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
    {
        // Compare the certificate's raw bytes to the expected bytes
        byte[] certificateRawBytes = cert.GetRawCertData();
        return StructuralComparisons.StructuralEqualityComparer.Equals(certificateRawBytes, certificateBytes);
    };

    return handler;
});



var conn = builder.Configuration.GetConnectionString("Default");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();


app.MapFallbackToFile("index.html");

app.Run();

public class CustomHttpClient : HttpClient
{
    public CustomHttpClient(HttpClientHandler handler) : base(handler)
    {
    }
}

public class CustomHttpClientHandler : HttpClientHandler
{
    public CustomHttpClientHandler()
    {
        // No need to instantiate the certificate validation here
        // It is done within the ConfigurePrimaryHttpMessageHandler
    }
}
