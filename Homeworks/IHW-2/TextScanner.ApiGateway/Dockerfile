# Use SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj file for dependency resolution
COPY "TextScanner.ApiGateway/TextScanner.ApiGateway.csproj" "TextScanner.ApiGateway/"

# Restore dependencies
RUN dotnet restore "TextScanner.ApiGateway/TextScanner.ApiGateway.csproj"

# Copy the source code for this service only
COPY "TextScanner.ApiGateway/" "TextScanner.ApiGateway/"

# Build the project
WORKDIR "/src/TextScanner.ApiGateway"
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
WORKDIR "/src/TextScanner.ApiGateway"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "TextScanner.ApiGateway.dll"]