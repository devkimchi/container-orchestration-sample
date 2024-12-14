using eShopLite.Store.DataEntities;

namespace eShopLite.Store.Endpoints;

public static class WeatherEndpoints
{
    private static string[] summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public static void MapWeatherEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/weatherforecast");

        group.MapGet("/", () =>
        {
            var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithTags("weather")
        .WithName("GetWeatherForecast")
        .Produces<WeatherForecast[]>(StatusCodes.Status200OK);
    }
}
