using eShopLite.ProductApi.Endpoints;
using eShopLite.ProductApi.Extensions;
using eShopLite.ProductData;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// builder.Services.AddDbContext<ProductDbContext>(options =>
// {
//     var connectionString = builder.Configuration.GetConnectionString("ProductsContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.");
//     options.UseSqlite(connectionString);
// });
builder.AddNpgsqlDbContext<ProductDbContext>("productsdb");

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

app.MapProductEndpoints();

app.CreateDbIfNotExists();

app.MapDefaultEndpoints();

app.Run();
