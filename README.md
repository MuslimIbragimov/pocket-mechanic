# Pocket Mechanic

Pocket Mechanic is a cross-platform mobile application built using .NET MAUI. This application helps you easily track all the necessary parts of your car's health including maintenance, tire monitoring, and daily driving analytics.

## Features

- **Cross-Platform Support**: Run the same codebase on multiple platforms (Android, iOS, MacCatalyst, and Windows).
- **Car Profile Management**: Create and manage profiles for your vehicles with make, model, year, and mileage tracking.
- **Tire Monitoring**: Track tire condition and estimate lifespan based on daily driving routes.
- **MVVM Architecture**: Utilizes the Model-View-ViewModel pattern for better separation of concerns.
- **Dependency Injection**: Configured services for better management and testing.

## Project Structure

- **src/PocketMechanic**: Contains the main application code.
  - **App.xaml**: Application layout and resources.
  - **App.xaml.cs**: Application initialization and main page setup.
  - **MainPage.xaml**: Layout for the main page.
  - **MainPage.xaml.cs**: Code-behind for the main page.
  - **MauiProgram.cs**: Application services configuration.
  - **Platforms**: Platform-specific implementations.
  - **Resources**: Fonts and styles used in the application.
  - **Views**: XAML views for the application.
  - **ViewModels**: Logic for the views.
  - **Models**: Data models used in the application.
  - **Services**: Classes for handling API calls and data retrieval.

- **tests/PocketMechanic.Tests**: Contains unit tests for the application.
  - **MainViewModelTests.cs**: Unit tests for the MainViewModel class.

## Getting Started

1. Clone the repository.
2. Open the solution file `PocketMechanic.sln` in your preferred IDE.
3. Restore the NuGet packages.
4. Run the application on your desired platform.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any enhancements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for details.