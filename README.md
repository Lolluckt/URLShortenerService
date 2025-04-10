# URLShortenerService

**URLShortenerService** is a web application designed to provide a URL shortening service. It allows users to convert long, complex URLs into shorter, more manageable links. The project includes a backend built with ASP.NET Core Web API and a frontend developed using TypeScript and SCSS.


## Live Demo

The application is deployed and accessible at:

[https://urlservice.netlify.app/](https://urlservice.netlify.app/)

> Note: When registering or logging in, the application might take up to a minute to respond. This is due to the free hosting service waking up from an idle state. Please be patient while the service starts.



## Project Structure

```
URLShortenerService/
├── Frontend/url-app     # Frontend application (Angular)
├── UrlService           # Backend service (ASP.NET Core Web API)
├── UrlTest              # Unit tests for backend
```

## Features

- URL shortening and redirection
- Simple web interface to input and retrieve URLs
- Basic URL validation and formatting
- Easily extendable for analytics or user management

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 or later)
- A SQL-based database (Server)

### Backend Setup

1. Navigate to the backend folder:

```bash
cd UrlService
```

2. Restore dependencies and run the API:

```bash
dotnet restore
dotnet run
```
3. Cofigure appsettings.json:
Example:
```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "your db connection"
  },
  "JwtSettings": {
    "Secret": "your JWT"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

### Frontend Setup

1. Navigate to the frontend directory:

```bash
cd Frontend/url-app
```

2. Install dependencies and start the development server:

```bash
npm install
npm run dev
```

The application will run at `http://localhost:5173`.

## Running Tests

Navigate to the test project directory and run the test suite:

```bash
cd UrlTest
dotnet test
```

## Technologies Used

- **Backend**: ASP.NET Core Web API, Entity Framework
- **Frontend**: Angular, TypeScript, HTML, SCSS
- **Testing**: xUnit

