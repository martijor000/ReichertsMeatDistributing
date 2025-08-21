using ReichertsMeatDistributing.Client.Services;
using ReichertsMeatDistributing.Server;
using ReichertsMeatDistributing.Server.Data;
using ReichertsMeatDistributing.Server.Services;
using ReichertsMeatDistributing.Server.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ProductSeederService>();
builder.Services.AddScoped<AdminAuthorizationService>();

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/api/admin/google-login";
    options.AccessDeniedPath = "/api/admin/access-denied";
    options.Cookie.Name = ".AspNetCore.AdminAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Extended to 7 days
    options.SlidingExpiration = true;
    options.Events.OnSigningOut = context =>
    {
        Console.WriteLine("=== COOKIE SIGN OUT EVENT ===");
        Console.WriteLine($"User signing out: {context.HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value}");
        return Task.CompletedTask;
    };
    options.Events.OnValidatePrincipal = context =>
    {
        Console.WriteLine("=== COOKIE VALIDATION EVENT ===");
        Console.WriteLine($"Validating user: {context.Principal?.FindFirst(ClaimTypes.Email)?.Value}");
        Console.WriteLine($"Is authenticated: {context.Principal?.Identity?.IsAuthenticated}");
        return Task.CompletedTask;
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    options.CallbackPath = "/signin-google";
    
    options.Events.OnCreatingTicket = context =>
    {
        // Get authorized emails from configuration
        var adminEmails = builder.Configuration.GetSection("AdminEmails").Get<List<string>>() ?? new List<string>
        {
            "jordan.martin.it@gmail.com",
            "tami.r.bickel@gmail.com"
        };
        
        var userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        
        Console.WriteLine($"=== GOOGLE AUTHENTICATION EVENT ===");
        Console.WriteLine($"Email attempting login: {userEmail}");
        Console.WriteLine($"Authorized emails: {string.Join(", ", adminEmails)}");
        
        if (string.IsNullOrEmpty(userEmail))
        {
            Console.WriteLine("AUTHENTICATION FAILED: No email found in Google claims");
            context.Fail("No email found in authentication response.");
            return Task.CompletedTask;
        }
        
        if (!adminEmails.Contains(userEmail))
        {
            Console.WriteLine($"AUTHENTICATION FAILED: Email '{userEmail}' not in admin list");
            context.Fail($"Access denied. The email '{userEmail}' is not authorized to access this application.");
            return Task.CompletedTask;
        }
        
        Console.WriteLine($"AUTHENTICATION SUCCESS: Email '{userEmail}' is authorized");
        Console.WriteLine($"=== END AUTHENTICATION EVENT ===");
        
        // Ensure the user is signed in with the cookie authentication scheme
        context.Success();
        return Task.CompletedTask;
    };
    
    options.Events.OnTicketReceived = context =>
    {
        Console.WriteLine("=== GOOGLE TICKET RECEIVED ===");
        Console.WriteLine($"Ticket received for user: {context.Principal?.FindFirst(ClaimTypes.Email)?.Value}");
        Console.WriteLine($"Authentication scheme: {context.Scheme.Name}");
        Console.WriteLine("=== END TICKET RECEIVED ===");
        return Task.CompletedTask;
    };
    

    

    
    options.Events.OnRemoteFailure = context =>
    {
        Console.WriteLine($"=== GOOGLE AUTHENTICATION FAILURE ===");
        Console.WriteLine($"Failure message: {context.Failure?.Message}");
        Console.WriteLine($"Failure type: {context.Failure?.GetType().Name}");
        Console.WriteLine($"=== END FAILURE EVENT ===");
        
        // Redirect to login with specific error message
        var errorMessage = "authentication_failed";
        if (context.Failure?.Message?.Contains("not authorized") == true)
        {
            errorMessage = "unauthorized";
        }
        else if (context.Failure?.Message?.Contains("No email found") == true)
        {
            errorMessage = "no_email";
        }
        
        context.Response.Redirect($"/login?error={errorMessage}");
        context.HandleResponse();
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new AdminRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

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

app.UseAuthentication();
// Temporarily disabled to debug authentication
// app.UseMiddleware<AdminValidationMiddleware>();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapFallbackToFile("index.html");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    // Seed products from ProductRepository
    var seeder = scope.ServiceProvider.GetRequiredService<ProductSeederService>();
    await seeder.SeedProductsAsync();
}

app.Run();

public class CustomHttpClient : HttpClient
{
    public CustomHttpClient(HttpClient httpClient) : base()
    {
        // Simplified constructor
    }
}
