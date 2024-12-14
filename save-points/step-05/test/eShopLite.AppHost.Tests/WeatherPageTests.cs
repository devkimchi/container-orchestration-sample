using Aspire.Hosting;

using FluentAssertions;

using Microsoft.Playwright;

namespace eShopLite.AppHost.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class WeatherPageTests : PageTest
{
    private DistributedApplication? _app;
    private ResourceNotificationService? _resource;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true,
    };

    [OneTimeSetUp]
    public async Task Setup()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.eShopLite_AppHost>().ConfigureAwait(false);
        appHost.Services.ConfigureHttpClientDefaults(builder => builder.AddStandardResilienceHandler());

        this._app = await appHost.BuildAsync().ConfigureAwait(false);

        this._resource = this._app.Services.GetRequiredService<ResourceNotificationService>();
        await this._app.StartAsync().ConfigureAwait(false);

        await this._resource!.WaitForResourceAsync("pg-pgadmin", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        await this._resource!.WaitForResourceAsync("productapi", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        await this._resource!.WaitForResourceAsync("weatherapi", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        await this._resource!.WaitForResourceAsync("webapp", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
    }

    [Test]
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_Table()
    {
        // Arrange
        var uri = $"{this._app!.GetEndpoint("webapp").ToString().TrimEnd('/')}/weather";
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        var table = Page.Locator("table.table");
        var content = await table.TextContentAsync().ConfigureAwait(false);

        // Assert
        table.Should().NotBeNull();
        content.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_TableRows()
    {
        // Arrange
        var uri = $"{this._app!.GetEndpoint("webapp").ToString().TrimEnd('/')}/weather";
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        var table = Page.Locator("table.table");
        var trs = table.Locator("tbody").Locator("tr");
        var count = await trs.CountAsync().ConfigureAwait(false);

        // Assert
        count.Should().Be(5);
    }

    [Test]
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_Date()
    {
        // Arrange
        var uri = $"{this._app!.GetEndpoint("webapp").ToString().TrimEnd('/')}/weather";
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var tomorrow = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        var fifth = tomorrow.AddDays(4);

        // Act
        var table = Page.Locator("table.table");
        var trs = table.Locator("tbody").Locator("tr");
        var first = await trs.First.Locator("td").First.TextContentAsync().ConfigureAwait(false);
        var last = await trs.Last.Locator("td").First.TextContentAsync().ConfigureAwait(false);

        // Assert
        first.Should().Be(tomorrow.ToString("yyyy-MM-dd"));
        last.Should().Be(fifth.ToString("yyyy-MM-dd"));
    }

    [OneTimeTearDown]
    public async Task Teardown()
    {
        await this._app!.DisposeAsync().ConfigureAwait(false);
    }
}
