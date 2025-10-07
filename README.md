# Task Tracker

A complete solution for tracking projects and tasks, built with .NET 8 . Includes REST API, JWT authentication, and a modern web UI.

---

## Prerequisites

- .NET 8
- Visual Studio 2022

---

## Quick Setup

### 1. Open Solution

Open `TaskTracker.sln` in Visual Studio 2022.

### 2. Configure Startup Projects

1. Right-click the Solution in Solution Explorer.
2. Select **Properties**.
3. Navigate to **Common Properties → Startup Project**.
4. Select **Multiple startup projects**.
5. Set both projects to **Start**:
   - `TaskTracker.API`
   - `TaskTracker.Web`
6. Click **OK**.

### 3. Database Setup

1. Open `appsettings.json` in the `TaskTracker.API` project.
2. Verify the database connection string is configured for SQL Server.
3. Open **Package Manager Console** (`View → Other Windows → Package Manager Console`).
4. Run the database migration command:
    - `update-database`
   - `Wait for database creation and initial seeding.`

### 4. Run the Application

- Press **F5** or click **Run**.
- API will start on: `https://localhost:7001`
- Web UI will start on: `https://localhost:7000`

### 5. First Time Setup

- Database will be automatically created and seeded.
- Default user created:  
  - **Username:** `admin`  
  - **Password:** `admin`

---

## Using the Application

### For Reviewers

1. **Explore Swagger Docs:**  
   [https://localhost:7001/swagger](https://localhost:7001/swagger)
2. **Login to Web App:**  
   [https://localhost:7000](https://localhost:7000)  
   Use credentials: `admin` / `admin`
3. **Test Features:**
   - Create projects and tasks
   - Browse & filter with pagination
   - Edit/Delete items
   - Register new users

### API Testing

- **Public endpoints:**  
  - `GET /api/projects`
  - `GET /api/tasks`
- **Protected endpoints:**  
  - All `POST`, `PUT`, `DELETE` (require login)

---

## Project Structure

- `TaskTracker.API` — REST API with JWT auth
- `TaskTracker.Web` — MVC Web UI consuming API
- `TaskTracker.Core` — Domain models & interfaces
- `TaskTracker.Infrastructure` — Data layer with EF Core

---

## Test Types Implemented

- ✅ **Unit Tests** — Domain logic, services, repositories
- ✅ **Integration Tests** — API endpoints, database operations
- ✅ **Authentication Tests** — JWT token validation
- ✅ **Validation Tests** — Input validation and error handling
- ✅ **Pagination Tests** — Filtering and pagination logic

---

## Technologies Used

- .NET 8, ASP.NET Core, MVC
- Entity Framework Core, SQL Server
- JWT Authentication
- Bootstrap 5, Swagger/OpenAPI, CORS 
- Clean Architecture, Repository Pattern


   
