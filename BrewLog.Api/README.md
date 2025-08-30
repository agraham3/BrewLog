# BrewLog API

A comprehensive coffee brewing tracking API built with .NET 8 Web API.

## Features

- Coffee bean inventory management
- Grind settings tracking
- Brewing equipment management
- Brew session logging with ratings and favorites
- Analytics and recommendations

## Technology Stack

- .NET 8 Web API
- Entity Framework Core 8
- SQLite (development) / SQL Server (production)
- AutoMapper for DTO mapping
- FluentValidation for input validation
- Swagger/OpenAPI for documentation

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQLite (for development)

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd BrewLog.Api
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to `http://localhost:5000` to access the Swagger UI.

### Database

The application uses SQLite for development with automatic database creation on first run. The database file will be created as `brewlog-dev.db` in the project root.

## API Documentation

When running in development mode, the Swagger UI is available at the root URL (`http://localhost:5000`) providing interactive API documentation.

## Project Structure

```
BrewLog.Api/
├── Controllers/       # API controllers
├── Data/              # Entity Framework DbContext
├── DTOs/              # Data Transfer Objects
├── Middleware/        # Custom middleware
├── Models/            # Domain models
├── Repositories/      # Data access layer
├── Services/          # Business logic layer
└── Validators/        # FluentValidation validators
```