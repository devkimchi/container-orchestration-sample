using eShopLite.Store.DataEntities;

namespace eShopLite.Store.ApiClients;

public class WeatherApiClient(HttpClient http)
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in http.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/api/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts ?? [];
    }
}
