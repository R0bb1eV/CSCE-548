# .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/BookTrackerApi
RUN dotnet publish BookTrackerApi.csproj -c Release -o /app/publish

# .NET runtime for hosting
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Bind to Render's $PORT at runtime (fallback to 10000 for local Docker runs).
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet BookTrackerApi.dll"]
