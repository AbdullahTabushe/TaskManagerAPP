using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManager.Frontend;
using TaskManager.Frontend.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client with base address for the API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7098/") 
});

// Add local storage
builder.Services.AddBlazoredLocalStorage();

// Add custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<AuthInitializationService>();

var app = builder.Build();

// Initialize authentication service
var authInitService = app.Services.GetRequiredService<AuthInitializationService>();
await authInitService.InitializeAsync();

await app.RunAsync();
