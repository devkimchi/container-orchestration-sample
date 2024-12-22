# Containerize a monolithic application with `docker init`

[`docker init`](https://docs.docker.com/reference/cli/docker/init/) is a CLI tool that helps you create a `Dockerfile` for your app. It detects the platform and the main project of your app and generates a `Dockerfile` for you. The downside of this approach is that you need to manually add referencing projects to the `Dockerfile` if your app has project references.

## Find the repository root

```bash
# Bazh/Zsh
REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
```

```powershell
# PowerShell
$REPOSITORY_ROOT = git rev-parse --show-toplevel
```

## `eShopLite.Store` in .NET

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

1. Open the `Dockerfile` and add the `RUN` instruction below:

    ```dockerfile
    # Find this line
    COPY --from=build /app .

    # Add the following RUN instruction
    RUN chown $APP_UID /app
    ```

    ```dockerfile
    # Find this line
    USER $APP_UID
    
    # Add the following RUN instruction
    RUN touch /app/Database.db
    ```

1. Build the container image.

    ```bash
    # To follow the build platform
    docker build -t eshoplite-store:latest .

    # To specify the target platform
    docker build --platform=linux/amd64 --build-arg TARGETARCH=amd64 -t eshoplite-store:latest .
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
