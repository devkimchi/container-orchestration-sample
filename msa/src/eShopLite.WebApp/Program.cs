using eShopLite.WebApp.ApiClients;
using eShopLite.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var config = builder.Configuration;

builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new(config["Services:Product:Http:0"]);
});

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new(config["Services:Weather:Http:0"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
