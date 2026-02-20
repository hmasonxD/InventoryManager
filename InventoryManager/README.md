# 📦 InventoryManager

A full-stack inventory management system I built to practice enterprise-level ASP.NET MVC development. The app handles product tracking, order processing, and stock management with role-based access control — the kind of system you'd actually see in a real warehouse or retail operation.

## Screenshots

![Dashboard](screenshots/dashboard.png)
![Products](screenshots/products.png)
![Categories](screenshots/categories.png)

## Features

- **Dashboard** with live stats, Chart.js charts, and low stock warnings
- **Product management** — full CRUD with search, filtering by category, and sortable columns
- **Order system** — users can place orders, pick products and quantities, and stock updates automatically
- **Categories & Suppliers** — organize products and track vendors
- **Role-based auth** — Admins get full access, Viewers can only browse and place orders
- **Low stock alerts** — flags products when inventory drops below a configurable threshold

## Tech Stack

| Layer          | Technology                  |
| -------------- | --------------------------- |
| Framework      | ASP.NET MVC (.NET 9)        |
| ORM            | Entity Framework Core       |
| Database       | SQL Server (LocalDB)        |
| Authentication | ASP.NET Identity with Roles |
| Frontend       | Bootstrap 5, Chart.js       |
| Architecture   | MVC Pattern                 |

## Database Schema

```
Category (1) ──── (*) Product (*) ──── (1) Supplier
                        │
                        │
               OrderItem (*) ──── (1) Order
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (comes with Visual Studio)

### Setup

```bash
git clone https://github.com/hmasonxD/InventoryManager.git
cd InventoryManager

# configure admin credentials
dotnet user-secrets set "AdminSettings:Email" "admin@inventory.com"
dotnet user-secrets set "AdminSettings:Password" "Admin123!"

# set up the database
dotnet ef database update

# run it
dotnet run
```

Then open `http://localhost:5097`

### Test Accounts

| Role   | Email                  | Password           |
| ------ | ---------------------- | ------------------ |
| Admin  | admin@inventory.com    | Admin123!          |
| Viewer | Register a new account | Any valid password |

## Roles & Permissions

| Action                 | Admin | Viewer |
| ---------------------- | ----- | ------ |
| View Dashboard         | ✅    | ✅     |
| Browse Products        | ✅    | ✅     |
| Search & Filter        | ✅    | ✅     |
| Place Orders           | ✅    | ✅     |
| Create / Edit / Delete | ✅    | ❌     |
| Manage Categories      | ✅    | ❌     |
| Manage Suppliers       | ✅    | ❌     |

## Project Structure

```
InventoryManager/
├── Constants/       # role name constants
├── Controllers/     # request handling + authorization
├── Data/            # DbContext, migrations, seed data
├── Models/          # entities + ViewModels
├── Views/           # Razor templates per controller
├── wwwroot/         # CSS, JS, static files
└── Program.cs       # startup + middleware config
```

## What I Learned Building This

- How the MVC pattern actually works end to end — routing, model binding, validation
- Entity Framework Core — code-first migrations, eager loading with `.Include()`, deferred execution with `.AsQueryable()`
- ASP.NET Identity — setting up roles, protecting routes with `[Authorize]`, seeding users on startup
- Dependency injection — why controllers ask for services instead of creating them
- Keeping secrets out of source code with `dotnet user-secrets`
