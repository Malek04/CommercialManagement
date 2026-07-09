# 🏢 CommercialManagement

CommercialManagement is a full-stack commercial management application that allows managing clients, products, and orders.

The project follows a layered architecture based on the **Repository Pattern**, using **ASP.NET Core Web API** for the backend and **Angular** for the frontend.

---

## Demo Video

▶️ [Watch the application demo video](https://drive.google.com/file/d/12PJmqc8KYnXjyLTgz44kP17-QE2ZnM_X/view)

---

## 📌 Overview

CommercialManagement helps businesses efficiently handle their commercial operations including client management, product catalog, and order processing.

---

## 🏗️ Architecture

The project is divided into multiple layers:

```
CommercialManagement
├── Back-end
│   ├── CommercialManagement.Api
│   │   ├── Controllers
│   │   └── Mappers
│   ├── CommercialManagement.Core
│   │   ├── Enums
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   └── Models
│   └── CommercialManagement.Infrastructure
│       ├── Database Context
│       ├── Repositories
│       └── Migrations
└── Front-end
    └── CommercialManagement
        ├── Components
        ├── Services
        ├── Models
        └── Pages
```

---

## 🛠️ Technologies Used

### Backend

| Technology              | Description                  |
|-------------------------|------------------------------|
| ASP.NET Core Web API    | REST API development         |
| .NET 8                  | Backend framework            |
| Entity Framework Core   | ORM                          |
| SQL Server              | Database management system   |
| Repository Pattern      | Data access layer            |
| AutoMapper              | Object mapping               |
| LINQ                    | Data querying                |
| Swagger                 | API documentation            |
| Dependency Injection    | Service management           |

### Frontend

| Technology     | Description             |
|----------------|-------------------------|
| Angular 21     | Frontend framework      |
| TypeScript     | Programming language    |
| HTML5          | Structure               |
| CSS3           | Styling                 |
| Bootstrap      | UI Framework            |
| RxJS           | Reactive programming    |

---

## 🗄️ Database

The application uses **Microsoft SQL Server**.

Entity Framework Core is used with the **Code First** approach.

Database migrations are located inside:

`CommercialManagement.Infrastructure/Migrations`

The migrations handle:
- Database creation
- Table creation
- Relationships
- Schema updates

---

## 📸 Database Structure

![Database Diagram](https://drive.google.com/uc?export=view&id=1NJ5oUy27Fa6WLE3AlHpo8lgRYcovTgRM)

---

## ⚙️ Installation Requirements

### Backend Requirements

- **.NET SDK**: .NET 8
- **SQL Server** + **SQL Server Management Studio (SSMS)**
- **Entity Framework Core Tools**
- **AutoMapper**

### Frontend Requirements

- **Node.js**: 24.14.1
- **npm**: 11.12.1
- **Angular CLI**: 21.2.18

---

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Malek04/CommercialManagement.git
cd CommercialManagement
```

### 2. Backend Setup

```bash
cd CommercialManagement.Api
```

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CommercialManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Restore packages and apply migrations:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

The API will be available at: `https://localhost:7110`  
Swagger UI: [https://localhost:7110/swagger/index.html](https://localhost:7110/swagger/index.html)

### 3. Frontend Setup

```bash
cd ../CommercialManagement.Angular
npm install
```

Update the API URL in `src/environments/environment.ts`:

```typescript
export const environment = {
  apiUrl: "https://localhost:7110/api"
};
```

Run the Angular application:

```bash
ng serve
# or
npm start
```

Frontend will be available at: **http://localhost:4200**

---

## 📝 Notes

- Make sure SQL Server is running and you have permissions to create databases.
- Update the connection string with your actual SQL Server instance name.
- For production, use environment-specific configuration and secure your connection strings.


