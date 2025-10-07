# Task Tracker - Complete Solution

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

## Quick Setup

1. **Open Solution**
   - Open `TaskTracker.sln` in Visual Studio 2022

2. **Set Multiple Startup Projects**
   - Right-click Solution → Properties
   - Set both `TaskTracker.API` and `TaskTracker.Web` as Startup Projects
   - Set Action: "Start" for both

3. **Run the Application**
   - Press F5 or click Run
   - API will start on: https://localhost:7001
   - Web UI will start on: https://localhost:7000

4. **First Time Setup**
   - Database will be automatically created and seeded
   - Default user created: `admin` / `password`

## Using the Application

### For Reviewers:
1. **Explore Swagger Docs** (Public): https://localhost:7001/swagger
2. **Login to Web App**: https://localhost:7000
   - Use: `admin` / `password`
3. **Test Features**:
   - Create projects and tasks
   - Use Browse & Filter with pagination
   - Edit/Delete items
   - Register new users

### API Testing:
- Public endpoints: GET /api/projects, GET /api/tasks
- Protected endpoints: All POST, PUT, DELETE (require login)

## Project Structure
- `TaskTracker.API` - REST API with JWT auth
- `TaskTracker.Web` - MVC UI consuming API
- `TaskTracker.Core` - Domain models & interfaces
- `TaskTracker.Infrastructure` - Data layer with EF Core
  
## Test Types Implemented:

✅ **Unit Tests** - Domain logic, services, repositories  
✅ **Integration Tests** - API endpoints, database operations  
✅ **Authentication Tests** - JWT token validation  
✅ **Validation Tests** - Input validation and error handling  
✅ **Pagination Tests** - Filtering and pagination logic  

## Key Testing Features:

- **In-memory database** for isolated testing
- **Mock dependencies** with Moq
- **Test fixtures** for shared setup
- **Theory tests** for multiple test cases
- **Integration testing** with WebApplicationFactory

## Technologies Used
- .NET 8, ASP.NET Core MVC, Entity Framework Core
- JWT Authentication, SQL Database
- Bootstrap 5, Swagger/OpenAPI
- Clean Architecture, Repository Pattern
