# Container Orchestration Sample

This provides sample apps using .NET and Java, with various usage with Dockerfile, without Dockerfile, container orchestration with Docker Compose and [.NET Aspire](https://aka.ms/dotnet-aspire), and deployment to a k8s cluster with .NET Aspire and Aspir8.

## Prerequisites

### .NET

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download/dotnet/9.0?WT.mc_id=dotnet-144884-juyoo)
- [Aspirate](https://github.com/prom3theu5/aspirational-manifests)

### Java

- [OpenJDK 17+](https://learn.microsoft.com/java/openjdk/download?WT.mc_id=dotnet-144884-juyoo)
- [Apache Maven 3.9.6+](https://maven.apache.org/download.cgi)

### Tooling

- [Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=dotnet-144884-juyoo)
  - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit&WT.mc_id=dotnet-144884-juyoo) extension
  - [Java Extension Pack](https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack&WT.mc_id=dotnet-144884-juyoo) extension
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

### Find the repository root

```bash
# Bazh/Zsh
REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
```

```powershell
# PowerShell
$REPOSITORY_ROOT = git rev-parse --show-toplevel
```

### Containerize a monolith app with `Dockerfile`

1. Move to the `monolith` directory.

    ```bash
    cd $REPOSITORY_ROOT/monolith
    ```

1. Run `docker init` to create a new `Dockerfile` for the app.

    ```bash
    docker init
    ```

   Follow the prompts to create a `Dockerfile` for the app.

   - Choose `ASP.NET Core` as the application platform
   - Choose `eShopLite.Store` as the main project
   - Enter `9.0` as the .NET SDK version
   - Enter `8080` as the host port number

1. Open the `Dockerfile` and add the `RUN` command below:

    ```dockerfile
    # Find this line
    COPY --from=build /app .

    # Add the following RUN command
    RUN chown $APP_UID /app
    ```

    ```dockerfile
    # Find this line
    USER $APP_UID
    
    # Add the following RUN command
    RUN touch /app/Database.db
    ```

1. Build the container image.

    ```bash
    docker build . -t eshoplite-store:latest
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 8080:8080 --name store eshoplite-store:latest
    ```

1. Open the browser and navigate to `http://localhost:8080` to see the web app running.

1. Stop and remove the container.

    ```bash
    docker stop store
    docker rm store --force
    ```

1. Delete the container image.

    ```bash
    docker rmi eshoplite-store:latest --force
    ```

### Containerize microservice apps without `Dockerfile`

#### `eShopLite.WeatherApi` in Java

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Make sure you have logged in Docker daemon to your preferred container registry. If not, run the following command:

    ```bash
    docker login
    ```

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Set the variables for building container image. Replace `{{YOUR_CONTAINER_REGISTRY}}` with your preferred container registry. For example, `my_container_registry.azurecr.io`. If you want to use Docker Hub, you can use your Docker Hub username.

    ```bash
    REGISTRY="{{YOUR_CONTAINER_REGISTRY}}"
    IMAGE="eshoplite-weather"
    TAG="latest"
    ```

    ```powershell
    $REGISTRY = "{{YOUR_CONTAINER_REGISTRY}}"
    $IMAGE = "eshoplite-weather"
    $TAG = "latest"
    ```

1. Build the container image.

    ```bash
    ./src/eShopLite.WeatherApi/mvnw clean package \
        -f ./src/eShopLite.WeatherApi/pom.xml \
        -Dimage="${REGISTRY}/${IMAGE}:${TAG}"
    ```

    ```powershell
    ./src/eShopLite.WeatherApi/mvnw clean package `
        -f ./src/eShopLite.WeatherApi/pom.xml `
        -Dimage="${REGISTRY}/${IMAGE}:${TAG}"
    ```

1. Pull the container image from the container registry.

    ```bash
    docker pull ${REGISTRY}/${IMAGE}:${TAG}
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 5050:5050 --name weather "${REGISTRY}/${IMAGE}:${TAG}"
    ```

1. Open the browser and navigate to `http://localhost:5050/api/weatherforecast` to see the app running.

1. Stop and remove the container.

    ```bash
    docker stop weather
    docker rm weather --force
    ```

1. Delete the container image.

    ```bash
    docker rmi "${REGISTRY}/${IMAGE}:${TAG}" --force
    ```

#### `eShopLite.ProductApi` in .NET

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Run the following `dotnet publish` command to build the app.

    ```bash
    dotnet publish ./src/eShopLite.ProductApi \
        -t:PublishContainer \
        --os linux --arch x64
    ```

    ```powershell
    dotnet publish ./src/eShopLite.ProductApi `
        -t:PublishContainer `
        --os linux --arch x64
    ```

1. If you want to change the base image to Ubuntu Chiseled image, use the following command.

    ```bash
    dotnet publish ./src/eShopLite.ProductApi \
        -t:PublishContainer \
        --os linux --arch x64 \
        -p:ContainerBaseImage=mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled \
        -p:ContainerRepository=eshoplite-product \
        -p:ContainerImageTag=latest
    ```

    ```powershell
    dotnet publish ./src/eShopLite.ProductApi `
        -t:PublishContainer `
        --os linux --arch x64 `
        -p:ContainerBaseImage=mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled `
        -p:ContainerRepository=eshoplite-product `
        -p:ContainerImageTag=latest
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 5051:8080 --name product eshoplite-product:latest
    ```

1. Open the browser and navigate to `http://localhost:5051/api/products` to see the app running.

1. Stop and remove the container.

    ```bash
    docker stop weather
    docker rm weather --force
    ```

1. Delete the container image.

    ```bash
    docker rmi "${REGISTRY}/${IMAGE}:${TAG}" --force
    ```

#### `eShopLite.WebApp` in .NET
