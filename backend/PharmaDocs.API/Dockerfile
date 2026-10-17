# Build context is the backend/ folder (not this project folder alone) because
# PharmaDocs.API references the Domain, Application, and Infrastructure projects.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first and restore, so the layer is cached unless deps change.
COPY PharmaDocs.Domain/PharmaDocs.Domain.csproj PharmaDocs.Domain/
COPY PharmaDocs.Application/PharmaDocs.Application.csproj PharmaDocs.Application/
COPY PharmaDocs.Infrastructure/PharmaDocs.Infrastructure.csproj PharmaDocs.Infrastructure/
COPY PharmaDocs.API/PharmaDocs.API.csproj PharmaDocs.API/
RUN dotnet restore PharmaDocs.API/PharmaDocs.API.csproj

COPY . .
RUN dotnet publish PharmaDocs.API/PharmaDocs.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Cloud Run convention: the container must listen on 8080.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PharmaDocs.API.dll"]
