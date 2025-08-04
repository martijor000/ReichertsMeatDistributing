using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Server;
using ReichertsMeatDistributing.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<AdminService>();

// Add SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Configure HttpClient without certificate validation for now
builder.Services.AddHttpClient<CustomHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://your-production-api-url/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.MapFallbackToFile("index.html");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

public class CustomHttpClient : HttpClient
{
    public CustomHttpClient(HttpClient httpClient) : base()
    {
        // Simplified constructor
    }
}
