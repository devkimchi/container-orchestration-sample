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

