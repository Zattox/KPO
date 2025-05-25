FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY "TextScanner.FileAnalysisService/TextScanner.FileAnalysisService.csproj" "TextScanner.FileAnalysisService/"
RUN dotnet restore "TextScanner.FileAnalysisService/TextScanner.FileAnalysisService.csproj"

COPY "TextScanner.FileAnalysisService/" "TextScanner.FileAnalysisService/"
WORKDIR "/src/TextScanner.FileAnalysisService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/TextScanner.FileAnalysisService"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
ENTRYPOINT ["dotnet", "TextScanner.FileAnalysisService.dll"]