## Инструкция по запуску приложения

1. **Требования**:
    - Установленный .NET 8.0 SDK.
    - Консоль (PowerShell, CMD или терминал Rider).

2. **Сборка и запуск**:
    - Откройте терминал в корневой папке проекта (`BankHSE`).
    - Выполните команды:
      ```bash
      dotnet restore
      dotnet run --project BankHSE/BankHSE.csproj
      ```
    - Приложение запустится, и вы увидите главное меню.
3. **Работа с тестами**
    - Для запуска тестов
      ```bash
      dotnet test BankHSE.Tests/BankHSE.Tests.csproj
      ```
    - Для генерации отчета о покрытии тестами
      ```bash
      dotnet test BankHSE.Tests/BankHSE.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=coverage.lcov
      reportgenerator -reports:coverage.lcov -targetdir:coverage-report -reporttypes:Html
      ```
    - Отчет будет в папке `coverage-report` (откройте `index.html`).
4. Импорт/экспорт:
   - Файлы для импорта/экспорта находятся в папке BankHSE/Data.
   - Укажите имя файла без пути (например, data.json) при выборе опции в меню.
5. Примечания:
   - Для корректной работы с русскими символами используется кодировка UTF-8.
   - Убедитесь, что файлы для импорта соответствуют ожидаемому формату (примеры: `data.csv`, `data.json`, `data.yaml`).