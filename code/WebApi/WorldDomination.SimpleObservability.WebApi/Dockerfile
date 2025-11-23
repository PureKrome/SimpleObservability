FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for Central Package Management.
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files.
COPY ["code/Core/WorldDomination.SimpleObservability/WorldDomination.SimpleObservability.csproj", "Core/WorldDomination.SimpleObservability/"]
COPY ["code/WebApi/WorldDomination.SimpleObservability.WebApi/WorldDomination.SimpleObservability.WebApi.csproj", "WebApi/WorldDomination.SimpleObservability.WebApi/"]

# Restore dependencies.
RUN dotnet restore "WebApi/WorldDomination.SimpleObservability.WebApi/WorldDomination.SimpleObservability.WebApi.csproj"

# Copy everything else.
COPY code/Core/WorldDomination.SimpleObservability/. Core/WorldDomination.SimpleObservability/
COPY code/WebApi/WorldDomination.SimpleObservability.WebApi/. WebApi/WorldDomination.SimpleObservability.WebApi/

# Build.
WORKDIR "/src/WebApi/WorldDomination.SimpleObservability.WebApi"
RUN dotnet build "WorldDomination.SimpleObservability.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorldDomination.SimpleObservability.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WorldDomination.SimpleObservability.WebApi.dll"]
