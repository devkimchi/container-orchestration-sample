# Container Orchestration Sample

This provides sample apps using .NET and Java, with various usage with Dockerfile, without Dockerfile, container orchestration with Docker Compose and [.NET Aspire](https://aka.ms/dotnet-aspire), and deployment to a k8s cluster with .NET Aspire and Aspir8.

## Prerequisites

### .NET

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download/dotnet/9.0?WT.mc_id=dotnet-157350-juyoo)
- [Aspirate](https://github.com/prom3theu5/aspirational-manifests)

### Java

- [OpenJDK 17+](https://learn.microsoft.com/java/openjdk/download?WT.mc_id=dotnet-157350-juyoo)
- [Apache Maven 3.9.6+](https://maven.apache.org/download.cgi)

### Tooling

- [Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=dotnet-157350-juyoo)
  - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit&WT.mc_id=dotnet-157350-juyoo) extension
  - [Java Extension Pack](https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack&WT.mc_id=dotnet-157350-juyoo) extension
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

- [Containerize a monolithic application with `docker init`](./monolith/)
- [Containerize msa applications with/without `Dockerfile`](./msa/)
- [Container orchestration with `docker network` and `docker compose`](./msa/#container-orchestration)
- [Container orchestration with .NET Aspire](./aspire/)

## Resources

- [Containerize .NET apps](https://aka.ms/dotnet/containerization)
- [Containerize Java apps](https://github.com/GoogleContainerTools/jib)
- [.NET Aspire](aka.ms/dotnet-aspire)
- [Aspirate from Scratch](https://github.com/devkimchi/aspir8-from-scratch)
