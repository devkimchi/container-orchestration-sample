using System.Net;
using System.Net.Http.Json;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using eShopLite.DataEntities;

using FluentAssertions;

namespace eShopLite.WeatherApi.Tests.Endpoints;

[TestFixture]
public class WeatherEndpointsTests
{
    private static HttpClient http = new();

    private IContainer? _container;

    [OneTimeSetUp]
    public void Setup()
    {
        this._container = new ContainerBuilder()
                              .WithImage("eshoplite-weatherapi-test:latest")
                              .WithPortBinding(8080, true)
                              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
                              .Build();
    }

    [SetUp]
    public async Task Init()
    {
        await this._container!.StartAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Given_Endpoint_When_Invoked_Then_It_Should_Return_OK()
    {
        // Arrange
        var uri = new UriBuilder(Uri.UriSchemeHttp, this._container!.Hostname, this._container!.GetMappedPublicPort(8080), "/api/weatherforecast").Uri;

        // Act
        var response = await http.GetAsync(uri).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Given_Endpoint_When_Invoked_Then_It_Should_Return_Collection()
    {
        // Arrange
        var uri = new UriBuilder(Uri.UriSchemeHttp, this._container!.Hostname, this._container!.GetMappedPublicPort(8080), "/api/weatherforecast").Uri;

        // Act
        var result = await http.GetFromJsonAsync<List<WeatherForecast>>(uri).ConfigureAwait(false);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().BeOfType<List<WeatherForecast>>();
    }

    [TearDown]
    public async Task Cleanup()
    {
        await this._container!.StopAsync().ConfigureAwait(false);
    }

    [OneTimeTearDown]
    public async Task Teardown()
    {
        await this._container!.DisposeAsync().ConfigureAwait(false);
    }
}
