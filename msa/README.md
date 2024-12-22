# Containerize msa applications with/without `Dockerfile`

## Find the repository root

```bash
# Bazh/Zsh
REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
```

```powershell
# PowerShell
$REPOSITORY_ROOT = git rev-parse --show-toplevel
```

## `eShopLite.ProductApi` in .NET with `Dockerfile`

Unlike the other microservice apps in this repository, this `eShopLite.ProductApi` app has to use `Dockerfile` for containerization because it requires the `RUN` instruction to create an SQLite database file inside the container.

1. Make sure you have been running Docker Desktop. If not, start Docker Desktop.

1. Move to the `msa` directory.

    ```bash
    cd $REPOSITORY_ROOT/msa
    ```

1. Build the container image.

    ```bash
    # To follow the build platform
    docker build -f Dockerfile.product -t eshoplite-product:latest .

    # To specify the target platform
    docker build --platform=linux/amd64 --build-arg TARGETARCH=amd64 -f Dockerfile.product -t eshoplite-product:latest .
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

## `eShopLite.WeatherApi` in Java without `Dockerfile`

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

## `eShopLite.WebApp` in .NET without `Dockerfile`

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

1. Open the browser and navigate to `http://localhost:5000` to see the app running. Make sure that both `/weather` and `/products` pages are not properly working, which is expected.

1. Stop and remove the container.

    ```bash
    docker stop webstore
    docker rm webstore --force
    ```

1. Delete the container image.

    ```bash
    docker rmi eshoplite-webstore:latest --force
    ```

# Container orchestration

## Container orchestration with `docker network`

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

1. Open the browser and navigate to `http://localhost:5000` to see the app running. Make sure this time both `/weather` and `/products` pages are properly showing contents.

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

## Container orchestration with `docker compose`

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
