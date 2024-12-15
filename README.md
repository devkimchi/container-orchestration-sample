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

### Containerize apps with `Dockerfile`

#### `eShopLite.Store` in .NET

[`docker init`](https://docs.docker.com/reference/cli/docker/init/) is a CLI tool that helps you create a `Dockerfile` for your app. It detects the platform and the main project of your app and generates a `Dockerfile` for you. The downside of this approach is that you need to manually add referencing projects to the `Dockerfile` if your app has project references.

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

#### `eShopLite.ProductApi` in .NET

[`MSBuild`](https://github.com/dotnet/msbuild) has a feature to build your .NET app containerized without a `Dockerfile`. The downside of this approach is that you can't add the `RUN` instruction. This `eShopLite.ProductApi` app has to create a database file when the app starts, which requires the `RUN` instruction.

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Build the container image.

    ```bash
    docker build . -f Dockerfile.product -t eshoplite-product:latest
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 5051:8080 --name product eshoplite-product:latest
    ```

1. Open the browser and navigate to `http://localhost:5051/api/products` to see the web app running.

1. Stop and remove the container.

    ```bash
    docker stop product
    docker rm product --force
    ```

1. Delete the container image.

    ```bash
    docker rmi eshoplite-product:latest --force
    ```

### Containerize microservice apps without `Dockerfile`

#### `eShopLite.WeatherApi` in Java

[`Jib`](https://github.com/GoogleContainerTools/jib) is a tool that builds a container image for your Java app without a `Dockerfile`. It's directly integrated into the Maven or Gradle build process. The downside of this approach is that you can't add the `RUN` instruction. If it's really necessary, use the `Dockerfile` or build a custom base image that has already applied the `RUN` instruction.

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
    ```

    ```powershell
    $REGISTRY = "{{YOUR_CONTAINER_REGISTRY}}"
    ```

1. Build the container image.

    ```bash
    ./src/eShopLite.WeatherApi/mvnw clean package \
        -f ./src/eShopLite.WeatherApi/pom.xml \
        -Dimage="$REGISTRY/eshoplite-weather:latest"
    ```

    ```powershell
    ./src/eShopLite.WeatherApi/mvnw clean package `
        -f ./src/eShopLite.WeatherApi/pom.xml `
        -Dimage="$REGISTRY/eshoplite-weather:latest"
    ```

1. Pull the container image from the container registry.

    ```bash
    docker pull "$REGISTRY/eshoplite-weather:latest"
    docker tag "$REGISTRY/eshoplite-weather:latest" eshoplite-weather:latest
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 5050:5050 --name weather "eshoplite-weather:latest"
    ```

1. Open the browser and navigate to `http://localhost:5050/api/weatherforecast` to see the app running.

1. Stop and remove the container.

    ```bash
    docker stop weather
    docker rm weather --force
    ```

1. Delete the container image.

    ```bash
    docker rmi eshoplite-weather:latest --force
    docker rmi "$REGISTRY/eshoplite-weather:latest" --force
    ```

#### `eShopLite.WebApp` in .NET

[`MSBuild`](https://github.com/dotnet/msbuild) has a feature to build your .NET app containerized without a `Dockerfile`. The downside of this approach is that you can't add the `RUN` instruction. If it's really necessary, use the `Dockerfile` or build a custom base image that has already applied the `RUN` instruction.

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Run the following `dotnet publish` command to build the app.

    ```bash
    dotnet publish ./src/eShopLite.WebApp \
        -t:PublishContainer \
        --os linux --arch x64
    ```

    ```powershell
    dotnet publish ./src/eShopLite.WebApp `
        -t:PublishContainer `
        --os linux --arch x64
    ```

1. If you want to change the base image to Ubuntu Chiseled image, use the following command.

    ```bash
    dotnet publish ./src/eShopLite.WebApp \
        -t:PublishContainer \
        --os linux --arch x64 \
        -p:ContainerBaseImage=mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled \
        -p:ContainerRepository=eshoplite-webstore \
        -p:ContainerImageTag=latest
    ```

    ```powershell
    dotnet publish ./src/eShopLite.WebApp `
        -t:PublishContainer `
        --os linux --arch x64 `
        -p:ContainerBaseImage=mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled `
        -p:ContainerRepository=eshoplite-webstore `
        -p:ContainerImageTag=latest
    ```

1. Run a container from the container image.

    ```bash
    docker run -d -p 5000:8080 --name webstore eshoplite-webstore:latest
    ```

1. Open the browser and navigate to `http://localhost:5000` to see the app running.

1. Stop and remove the container.

    ```bash
    docker stop webstore
    docker rm webstore --force
    ```

1. Delete the container image.

    ```bash
    docker rmi eshoplite-webstore:latest --force
    ```

### Container Orchestration

#### `docker network`

1. Create a new bridge network.

    ```bash
    docker network create eshoplite
    ```

1. Run containers from each container image with the network attached

    ```bash
    docker run -d -p 5000:8080 --network eshoplite --network-alias webstore --name webstore eshoplite-webstore:latest
    docker run -d -p 5050:5050 --network eshoplite --network-alias weather --name weather eshoplite-weather:latest
    docker run -d -p 5051:8080 --network eshoplite --network-alias product --name product eshoplite-product:latest
    ```

1. Open the browser and navigate to `http://localhost:5000` to see the app running.

1. Delete containers and network

    ```bash
    docker stop webstore
    docker rm webstore --force
    
    docker stop weather
    docker rm weather --force
    
    docker stop product
    docker rm product --force
    
    docker network rm eshoplite --force
    ```

#### `docker compose`

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Run the following command to run all the containers.

    ```bash
    docker compose up
    ```

1. Open the browser and navigate to `http://localhost:5000` to see the app running.

1. Stop and remove the containers.

    ```bash
    docker compose down
    ```

#### .NET Aspire

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `aspire` directory.

    ```bash
    cd $REPOSITORY_ROOT/aspire
    ```

1. Run the following command to let .NET Aspire orchestrate all apps at once.

    ```bash
    dotnet watch run --project ./src/eShopLite.AppHost
    ```

1. Checkout the dashboard.
1. Open the browser and navigate to `http://localhost:5000` to see the app running.

### Deployment to a Kubernetes Cluster

#### .NET Aspire and Aspir8

1. Make sure you have been running Docker Desktop and enabled Kubernetes. If not, start Docker Desktop and Kubernetes.

1. Move to the `aspire` directory.

    ```bash
    cd $REPOSITORY_ROOT/aspire
    ```

1. Follow the instructions in order:

   1. [Setup local Kubernetes dashboard](https://github.com/devkimchi/aspir8-from-scratch?tab=readme-ov-file#kubernetes-dashboard-setup)
   1. [Deploy the apps to the local Kubernetes cluster](https://github.com/devkimchi/aspir8-from-scratch?tab=readme-ov-file#aspire-flavoured-app-deployment-to-kubernetes-cluster-through-aspir8)

## Resources

- [Containerize .NET apps](https://aka.ms/dotnet/containerization)
- [Containerize Java apps](https://github.com/GoogleContainerTools/jib)
- [.NET Aspire](aka.ms/dotnet-aspire)
- [Aspirate from Scratch](https://github.com/devkimchi/aspir8-from-scratch)
