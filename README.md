# LogiTrack OMS

LogiTrack OMS (Order Management System) is a logistics software platform designed to manage inventory items and customer orders across multiple fulfillment centers. This project is built with ASP.NET Core Web API and Entity Framework Core (EF Core) using SQLite as the database.

## Features

- Manage inventory items with location and quantity tracking
- Create and manage customer orders
- Relational database integration with EF Core (SQLite)
- Seed and test data with console output
- Extensible for future API, security, and performance enhancements

## Project Structure

```
LogiTrack/
├── Models/
│   ├── InventoryItem.cs
│   ├── Order.cs
│   └── LogiTrackContext.cs
├── Program.cs
└── README.md
```

### Components

- **InventoryItem.cs**: Represents an inventory item with properties for ID, name, quantity, location, and EF Core navigation properties.
- **Order.cs**: Represents a customer order, including order ID, customer name, date placed, and a list of inventory items. Includes methods to add/remove items and summarize the order.
- **LogiTrackContext.cs**: EF Core DbContext for managing database access and relationships.
- **Program.cs**: Seeds the database with test data and demonstrates basic functionality via console output.

## Setup Instructions

1. **Clone the repository**
   ```sh
   git clone <your-repo-url>
   cd LogiTrack
   ```

2. **Install dependencies**
   ```sh
   dotnet restore
   ```

3. **Add EF Core packages (if not already present)**
   ```sh
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

4. **Create and update the database**
   ```sh
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```sh
   dotnet run
   ```

   The application will seed the database with a sample inventory item (if none exist) and print inventory information to the console.

## Running Tests

Before running tests, set the required environment variables in your shell:

```sh
export Jwt__Key=supersecretkey1234supersecretkey1234supersecretkey1234supersecretkey1234
export Jwt__Issuer=logitrack
export Jwt__Audience=logitrack
export USE_INMEMORY_DB=1
dotnet test
```

This ensures your tests use the correct JWT key and configuration for authentication and in-memory database.

## Development Notes

- The project uses a one-to-many relationship between `Order` and `InventoryItem` via EF Core.
- Data annotations are used for primary keys and required fields.
- The code is ready for extension with API endpoints, authentication, and further business logic.

## Next Steps

- Implement API controllers for inventory and orders.
- Add authentication and authorization.
- Optimize performance for large datasets.
- Extend business logic as needed.

---

## Q&A

**Describe your API project and its key features.**  
LogiTrack OMS is a RESTful API for managing inventory and customer orders in logistics operations. Built with ASP.NET Core and EF Core, it supports user authentication (JWT), role-based authorization, and CRUD operations for inventory and orders. The API features robust validation, error handling, and supports both in-memory and SQLite databases for flexible development and testing. Integrated Swagger UI provides interactive documentation, and the project includes comprehensive integration tests and code coverage reporting to ensure reliability and maintainability.

**What were the major challenges you faced, and how did you overcome them?**  
Major challenges included ensuring secure authentication and authorization, managing environment-specific configurations, and achieving reliable test coverage in various environments like Codespaces and CI. These were overcome by using JWT for authentication, conditional compilation for environment-specific logic, and flexible test assertions to accommodate different deployment scenarios. The use of `.env` files, robust error handling, and comprehensive integration tests helped ensure the API is secure, configurable, and reliable across development, testing, and production.

**How did you implement key components like business logic, data persistence, and state management?**  
Business logic is encapsulated within API controllers, handling validation, error responses, and entity relationships. Data persistence is managed using Entity Framework Core, with a DbContext that supports both SQLite and in-memory databases for different environments. State management is handled by the database for persistent data, while ASP.NET Core’s in-memory caching is used for frequently accessed data to improve performance. The architecture is modular, making it easy to extend business logic and integrate additional services as needed.

**What security measures did you implement?**  
Security is enforced through JWT-based authentication and role-based authorization, ensuring only authenticated users can access protected endpoints and only users with appropriate roles (e.g., Manager) can perform sensitive actions like deleting orders. Environment variables and `.env` files are used to manage secrets securely, and `[AllowAnonymous]` is only enabled in debug/test builds. All sensitive endpoints require authentication in production, and best practices are followed to avoid exposing secrets or sensitive data in source code or configuration files.

**How did you manage caching and optimize performance?**  
Caching is managed using ASP.NET Core’s in-memory caching, which reduces database load for frequently accessed endpoints. Performance is further optimized by designing efficient database queries, leveraging EF Core’s change tracking, and minimizing unnecessary data retrieval. Integration tests include checks for caching effectiveness and response times. The API is structured to allow easy scaling and further optimization, ensuring it remains responsive and efficient even as data volume grows.

---

**LogiTrack OMS** is designed for modularity and scalability, making it suitable for real-world logistics and order management scenarios.

