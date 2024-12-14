var builder = DistributedApplication.CreateBuilder(args);

var productsdb = builder.AddPostgres("pg")
                        .WithPgAdmin()
                        .AddDatabase("productsdb");

var productapi = builder.AddProject<Projects.eShopLite_ProductApi>("productapi")
                        .WithReference(productsdb)
                        .WaitFor(productsdb);

var weatherapi = builder.AddProject<Projects.eShopLite_WeatherApi>("weatherapi");

builder.AddProject<Projects.eShopLite_WebApp>("webapp")
       .WithExternalHttpEndpoints()
       .WithReference(productapi)
       .WithReference(weatherapi)
       .WaitFor(productapi)
       .WaitFor(weatherapi);

builder.Build().Run();
