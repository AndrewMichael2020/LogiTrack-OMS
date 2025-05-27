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

## Development Notes

- The project uses a one-to-many relationship between `Order` and `InventoryItem` via EF Core.
- Data annotations are used for primary keys and required fields.
- The code is ready for extension with API endpoints, authentication, and further business logic.


---

**LogiTrack OMS** is designed for modularity and scalability, making it suitable for real-world logistics and order management scenarios.
