# TODO: Replace the placeholder image names with valid .NET images that match your target framework.
FROM REPLACE_WITH_DOTNET_SDK_IMAGE AS build
WORKDIR /src
COPY . .
RUN dotnet publish BookTrackerApi.csproj -c Release -o /app/publish

# TODO: Replace the placeholder image name below, too.
FROM REPLACE_WITH_DOTNET_RUNTIME_IMAGE AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "BookTrackerApi.dll"]
