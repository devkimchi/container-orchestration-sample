using eShopLite.WeatherApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Redirect to OpenAPI endpoint
    app.MapGet("/", () =>
    {
        return Results.Redirect("/openapi/v1.json");
    })
    .ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.MapWeatherEndpoints();

app.MapDefaultEndpoints();

app.Run();
