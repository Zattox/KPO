using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Helpers.Menus;

// Configurate DI-container
var serviceProvider = ServiceConfiguration.ConfigureServices();

// Get MainMenu
var mainMenu = serviceProvider.GetRequiredService<MainMenu>();

// Start app
mainMenu.ShowMenu();