var builder = DistributedApplication.CreateBuilder(args);

var productsdb = builder.AddPostgres("pg")
                        .WithPgAdmin()
                        .AddDatabase("productsdb");

var productapi = builder.AddProject<Projects.eShopLite_ProductApi>("product")
                        .WithReference(productsdb)
                        .WaitFor(productsdb);

var weatherapi = builder.AddSpringApp("weather",
                            workingDirectory: "../eShopLite.WeatherApi",
                            new JavaAppExecutableResourceOptions()
                            {
                                ApplicationName = "target/weatherapi-0.0.1-SNAPSHOT.jar",
                                Port = 5050,
                                OtelAgentPath = "../../../agents",
                            })
                        .PublishAsDockerFile(
                            [
                                new DockerBuildArg("JAR_NAME", "weatherapi-0.0.1-SNAPSHOT.jar"),
                                new DockerBuildArg("AGENT_PATH", "/agents"),
                                new DockerBuildArg("SERVER_PORT", "5050"),
                            ]);

builder.AddProject<Projects.eShopLite_WebApp>("webstore")
       .WithExternalHttpEndpoints()
       .WithReference(productapi)
       .WithReference(weatherapi)
       .WaitFor(productapi)
       .WaitFor(weatherapi);

builder.Build().Run();
