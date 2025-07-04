# Task Manager API

A comprehensive task management system built with .NET 8.0, featuring both API and client applications.

## Project Structure

- **TaskManagerAPI**: The main backend API project
- **TaskManager.Client**: Blazor WebAssembly client application
- **TaskManager.Frontend**: Alternative frontend implementation
- **TaskManagerAPI.Tests**: Unit tests for the API

## Features

- User Authentication and Authorization
- Project Management
- Task Management
- Activity Logging
- Admin User Management
- Dashboard with Statistics

## Technologies Used

- .NET 8.0
- Entity Framework Core
- Blazor WebAssembly
- SQL Server
- Bootstrap

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
2. Navigate to the project directory
3. Update the connection string in `appsettings.json`
4. Run migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the API:
   ```bash
   cd TaskManagerAPI
   dotnet run
   ```
6. Run the Client:
   ```bash
   cd TaskManager.Client
   dotnet run
   ```

## Contributing

1. Fork the project
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## License

This project is licensed under the MIT License. 