# Container orchestration with .NET Aspire

[.NET Aspire](https://aka.ms/dotnet-aspire) provides an easy and simple way to orchestrate containers without using `docker network` or `docker compose`. It even provides a way to deploy all the apps to a Kubernetes cluster.

## Find the repository root

```bash
# Bazh/Zshs
REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
```

```powershell
# PowerShell
$REPOSITORY_ROOT = git rev-parse --show-toplevel
```

## Container orchestration with .NET Aspire through `eShopLite.AppHost`

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

## Deployment to a Kubernetes Cluster through .NET Aspire

1. Make sure you have been running Docker Desktop and enabled Kubernetes. If not, start Docker Desktop and Kubernetes.

1. Move to the `aspire` directory.

    ```bash
    cd $REPOSITORY_ROOT/aspire
    ```

1. Follow the instructions in order:

   1. [Setup local Kubernetes dashboard](https://github.com/devkimchi/aspir8-from-scratch?tab=readme-ov-file#kubernetes-dashboard-setup)
   1. [Deploy the apps to the local Kubernetes cluster](https://github.com/devkimchi/aspir8-from-scratch?tab=readme-ov-file#aspire-flavoured-app-deployment-to-kubernetes-cluster-through-aspir8)
