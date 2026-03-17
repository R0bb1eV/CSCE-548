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
ENV ASPNETCORE_URLS=http://+:${PORT}
ENTRYPOINT ["dotnet", "BookTrackerApi.dll"]
