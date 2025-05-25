FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY "TextScanner.FileStoringService/TextScanner.FileStoringService.csproj" "TextScanner.FileStoringService/"
RUN dotnet restore "TextScanner.FileStoringService/TextScanner.FileStoringService.csproj"

COPY "TextScanner.FileStoringService/" "TextScanner.FileStoringService/"
WORKDIR "/src/TextScanner.FileStoringService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/TextScanner.FileStoringService"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/Files
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
ENTRYPOINT ["dotnet", "TextScanner.FileStoringService.dll"]