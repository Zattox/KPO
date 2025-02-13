using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Helpers.Menus;

// Настройка DI-контейнера
var serviceProvider = ServiceConfiguration.ConfigureServices();

// Получение экземпляра MainMenu
var mainMenu = serviceProvider.GetRequiredService<MainMenu>();

// Запуск главного меню
mainMenu.ShowMenu();