using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using FluentAssertions;

using Microsoft.Playwright;

namespace eShopLite.WebApp.Tests.Components.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductsPageTests : PageTest
{
    private INetwork? _network;
    private ContainerBuilder? _productApiContainerBuilder;
    private ContainerBuilder? _webAppContainerBuilder;
    private IContainer? _productApiContainer;
    private IContainer? _webAppContainer;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true,
    };

    [OneTimeSetUp]
    public void Setup()
    {
        this._network = new NetworkBuilder()
                            .WithName(Guid.NewGuid().ToString("D"))
                            .Build();

        this._productApiContainerBuilder = new ContainerBuilder()
                                               .WithImage("eshoplite-productapi-test:latest")
                                               .WithName("productapi")
                                               .WithNetwork(this._network)
                                               .WithNetworkAliases("productapi")
                                               .WithPortBinding(8080, true)
                                               .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080));

        this._webAppContainerBuilder = new ContainerBuilder()
                                           .WithImage("eshoplite-webapp-test:latest")
                                           .WithNetwork(this._network)
                                           .WithNetworkAliases("webapp")
                                           .WithPortBinding(8080, true)
                                           .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080));
    }

    [SetUp]
    public async Task Init()
    {
        this._productApiContainer = this._productApiContainerBuilder!.Build();
        this._webAppContainer = this._webAppContainerBuilder!.Build();

        await this._network!.CreateAsync().ConfigureAwait(false);

        await this._productApiContainer!.StartAsync().ConfigureAwait(false);
        await this._webAppContainer!.StartAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_Table()
    {
        // Arrange
        var uri = new UriBuilder(Uri.UriSchemeHttp, this._webAppContainer!.Hostname, this._webAppContainer!.GetMappedPublicPort(8080), "/products").Uri.ToString();
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
        var uri = new UriBuilder(Uri.UriSchemeHttp, this._webAppContainer!.Hostname, this._webAppContainer!.GetMappedPublicPort(8080), "/products").Uri.ToString();
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        var table = Page.Locator("table.table");
        var trs = table.Locator("tbody").Locator("tr");
        var count = await trs.CountAsync().ConfigureAwait(false);

        // Assert
        count.Should().Be(9);
    }

    [Test]
    public async Task Given_PageUrl_When_Invoked_Then_It_Should_Return_Date()
    {
        // Arrange
        var uri = new UriBuilder(Uri.UriSchemeHttp, this._webAppContainer!.Hostname, this._webAppContainer!.GetMappedPublicPort(8080), "/products").Uri.ToString();
        await Page.GotoAsync(uri);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        var table = Page.Locator("table.table");
        var trs = table.Locator("tbody").Locator("tr");
        var first = await trs.First.Locator("td").First.TextContentAsync().ConfigureAwait(false);
        var last = await trs.Last.Locator("td").First.TextContentAsync().ConfigureAwait(false);

        // Assert
        first.Should().Be("1");
        last.Should().Be("9");
    }

    [TearDown]
    public async Task Cleanup()
    {
        await this._webAppContainer!.StopAsync().ConfigureAwait(false);
        await this._productApiContainer!.StopAsync().ConfigureAwait(false);
        await this._network!.DeleteAsync().ConfigureAwait(false);

        await this._webAppContainer!.DisposeAsync().ConfigureAwait(false);
        await this._productApiContainer!.DisposeAsync().ConfigureAwait(false);
    }

    [OneTimeTearDown]
    public async Task Teardown()
    {
        await this._network!.DisposeAsync().ConfigureAwait(false);
    }
}
