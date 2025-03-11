<h1 align="center">
  eCommerce
</h1>

<div align="center">
  <img src="https://github.com/oistikbal/eCommerce/actions/workflows/dotnet.yml/badge.svg?" alt="Status">
    <img src="https://oistikbal.github.io/eCommerce/badge_linecoverage.svg" alt="Status">
    <img src="https://oistikbal.github.io/eCommerce/badge_branchcoverage.svg" alt="Branch">
    <img src="https://oistikbal.github.io/eCommerce/badge_methodcoverage.svg" alt="Method">
    <img src="https://oistikbal.github.io/eCommerce/badge_fullmethodcoverage.svg" alt="Full Method">
</div>

<p align="cener">
This is an eCommerce project built with .NET 9.0 using a microservices architecture.
</p>

## Technologies
- API Gateway (YARP)
- gRPC
- RabbitMQ
- PostgreSQL
- Entity Framework Core
- Health Checks and Health UI
- Coverage Report

## Prerequisites
- .NET 9.0
- Docker

## Build

**Clone the repository:**
   ```bash
   git clone https://github.com/oistikbal/eCommerce.git
   cd eCommerce
   ```

**Install dependencies:**
   Ensure that you have the .NET 9.0 SDK installed. Run the following command:
   ```bash
   dotnet restore
   ```

**Install dependencies:**
   Ensure that you have the .NET 9.0 SDK installed. Run the following command:
   ```bash
   dotnet restore
   ```

**Database setup:**
   Make sure PostgreSQL is running.
   Run migrations:
   ```bash
   dotnet ef database update
   ```

**Run RabbitMQ**:
Ensure that Docker is installed and running.
Start RabbitMQ using Docker:
   ```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
   ```

**Run the application:**
   ```bash
   dotnet run
   ```

## Tests
Tests are executed using Testcontainers, which requires Docker to be running.
Before running tests, ensure Docker is open and running.
Run tests with:
```bash
dotnet test
```



## License
This project is licensed under the [MIT License](LICENSE).