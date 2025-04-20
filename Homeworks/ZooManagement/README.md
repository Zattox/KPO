# Отчёт по Домашнему заданию №2

## Получение результатов тестового покрытия

Для анализа тестового покрытия проекта были выполнены следующие шаги

1. Запуск тестов со сбором покрытия
   ```bash
   dotnet test ZooManagement.Tests/ZooManagement.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage.lcov
   ```
2. Генерация HTML-отчёта
   ```bash
   reportgenerator -reports:ZooManagement.Tests/coverage.lcov -targetdir:ZooManagement.Tests/CoverageReport -reporttypes:Html
   ```
3. Отчёт будет доступен по пути `ZooManagement.Tests/CoverageReport/index.html`

## Реализованный функционал и классы/модули

1. Добавить/удалить животное
   - Добавление животного реализовано в `AnimalsController` через метод POST (`/api/animals`). 
   Принимает `AnimalDto`, создаёт `Animal` с использованием `Value Objects`, добавляет его в репозиторий через `IAnimalRepository.Add`.
   - Удаление животного реализовано в `AnimalsController` через метод DELETE (`/api/animals/{id}`).
   Удаляет животное через `IAnimalRepository.Remove`.
2. Добавить/удалить вольер
    - Добавление вольера реализовано в `EnclosuresController` через метод POST (`/api/enclosures`).
      Принимает `EnclosureDt`, создаёт `Enclosure` с использованием `Value Objects`, добавляет в репозиторий через `IEnclosureRepository.Add`.
    - Удаление вольера реализовано в `EnclosuresController` через метод DELETE (`/api/enclosures/{id}`).
    Удаляет вольер через `IEnclosureRepository.Remove`
3. Переместить животное в другой вольер
    - Перемещение животного реализовано в AnimalsController через метод POST (`/api/animals/{id}/transfer`).
      Принимает `id` животного и `enclosureId`, вызывает `AnimalTransferService.TransferAnimal`, возвращает `AnimalMovedEvent`. 
      `AnimalTransferService` оркестрирует процесc.
4. Просмотреть/добавить/изменить расписание кормления
    - Просмотр расписания реализован в `FeedingSchedulesController` через метод GET (`/api/feedingschedules`).
      Возвращает список `FeedingScheduleDto`, используя `IFeedingScheduleRepository.GetAll`.
    - Добавление расписания реализовано в `FeedingSchedulesController` через метод POST (`/api/feedingschedules`).
      Принимает `FeedingScheduleDto`, проверяет существование животного и соответствие `FoodType` с `FavoriteFood` через `IAnimalRepository`, создаёт `FeedingSchedule`, добавляет через `IFeedingScheduleRepository.Add`.
    - Изменение расписания реализовано в `FeedingSchedulesController` через метод PATCH (`/api/feedingschedules/{id}`).
      Принимает `FeedingScheduleDto`, проверяет существование расписания и животного, обновляет расписание через `FeedingSchedule.Update`, сохраняет через `IFeedingScheduleRepository.Update`.
5. Выполнить кормление
    - Завершение кормления реализовано в `FeedingSchedulesController` через метод POST (`/api/feedingschedules/{id}/complete`).
      Вызывает `FeedingOrganizationService.CompleteFeeding`, возвращает `FeedingTimeEvent`.
6. Получить статистику зоопарка
    - Получение статистики реализовано в `StatisticsController` через метод GET (`/api/statistics`).
      Возвращает `ZooStatisticsDto` с данными о количестве животных и свободных вольеров.

## Применённые концепции DDD

1. Entities
   - Реализовано в `Animal`, `Enclosure`, `FeedingSchedule` (`Domain.Entities`)
2. Value Objects
   - Использованы для инкапсуляции валидации и неизменяемых данных (`AnimalName`, `BirthDate`, `EnclosureSize`, `EnclosureCapacity`, `FeedingTime`)
3. Domain Events
   - Реализовано в `AnimalMovedEvent` и `FeedingTimeEvent` (`Domain.Events`). 
   `AnimalMovedEvent` генерируется при перемещении животного.
   `FeedingTimeEvent` генерируется при завершении кормления.
4. Repositories
   - Интерфейсы `IAnimalRepository`, `IEnclosureRepository`, `IFeedingScheduleRepository` (`Application.Abstractions`) определяют доступ к данным
   - Реализация в `InMemoryAnimalRepository`, `InMemoryEnclosureRepository`, `InMemoryFeedingScheduleRepository` скрывает детали хранения, работая с доменными сущностями.
   
## Принципы Clean Architecture

1. Dependency Rule
   - `Domain` (сущности, Value Objects, события) не имеет зависимостей.
   - `Application` (DTO, сервисы, интерфейсы репозиториев) зависит только от `Domain`.
   - `Infrastructure` (`InMemoryRepository`) зависит от `Application` (интерфейсы) и `Domain` (сущности).
   - `Presentation` (контроллеры) зависит от `Application` (DTO, сервисы, интерфейсы).

2. Separation of Concerns
   - `Domain`: Содержит бизнес-логику
   - `Application`: Оркестрирует бизнес-логику через сервисы и предоставляет DTO для передачи данных.
   - `Infrastructure`: Реализует доступ к данным
   - `Presentation`: Обрабатывает HTTP-запросы и ответы

3. Interface Segregation
   - Интерфейсы репозиториев (`IAnimalRepository`, `IEnclosureRepository`, `IFeedingScheduleRepository`) разделены по сущностям, предоставляя минимальный контракт для каждой.
