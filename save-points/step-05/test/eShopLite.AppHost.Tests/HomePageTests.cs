using Aspire.Hosting;

using FluentAssertions;

using Microsoft.Playwright;

namespace eShopLite.AppHost.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomePageTests : PageTest
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
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_Heading1()
    {
        // Arrange
        var uri = this._app!.GetEndpoint("webapp").ToString();
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        var h1 = Page.Locator("h1");
        var content = await h1.TextContentAsync().ConfigureAwait(false);

        // Assert
        content.Should().Be("Hello, world!");
    }

    [OneTimeTearDown]
    public async Task Teardown()
    {
        await this._app!.DisposeAsync().ConfigureAwait(false);
    }
}
