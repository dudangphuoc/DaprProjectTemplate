# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["DXDaprProjectTemplate/src/templates/src/DXProjectName.Api/nuget.config", "DXDaprProjectTemplate/src/templates/src/DXProjectName.Api/"]
COPY ["DXDaprProjectTemplate/src/templates/src/DXProjectName.Api/DXProjectName.Api.csproj", "DXDaprProjectTemplate/src/templates/src/DXProjectName.Api/"]
COPY ["DXDaprProjectTemplate/src/templates/src/DXProjectName.Infrastructure/DXProjectName.Infrastructure.csproj", "DXDaprProjectTemplate/src/templates/src/DXProjectName.Infrastructure/"]
COPY ["DXDaprProjectTemplate/src/templates/src/DXProjectName.Domain/DXProjectName.Domain.csproj", "DXDaprProjectTemplate/src/templates/src/DXProjectName.Domain/"]
RUN dotnet restore "./DXDaprProjectTemplate/src/templates/src/DXProjectName.Api/DXProjectName.Api.csproj"
COPY . .
WORKDIR "/src/DXDaprProjectTemplate/src/templates/src/DXProjectName.Api"
RUN dotnet build "./DXProjectName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DXProjectName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DXProjectName.Api.dll"]