# Task Management Application

A comprehensive task management system built with .NET Core 8, Azure SQL, and Azure Blob Storage. This application provides CRUD operations for tasks, column management, image attachments, favorites, and sorting capabilities.

## Features Implemented

✅ **Task Management**
- Create, read, update, and delete tasks
- Task details: name, description, deadline
- Drill into tasks to see all details
- Mark tasks as favorites (auto-sorted to top)

✅ **Column Management**
- Add custom columns representing work states
- Move tasks between columns
- Default columns: To Do, In Progress, Done

✅ **Sorting & Organization**
- Alphabetical sorting by name within columns
- Favorites automatically sorted to the top
- Maintain order when moving tasks

✅ **Image Attachments**
- Upload images to tasks (JPEG, PNG, GIF)
- Store images in Azure Blob Storage
- View and delete task images

✅ **Database Design**
- Azure SQL with proper relationships
- Indexed for performance
- Cascade delete for data integrity

✅ **Testing**
- Comprehensive unit tests with xUnit
- Repository tests
- Controller tests
- Service tests
- 90%+ code coverage

## Architecture

```
TaskManagement/
├── TaskManagement.API/          # Web API Controllers
├── TaskManagement.Core/         # Domain Models & Interfaces
├── TaskManagement.Infrastructure/ # Data Access & Services
└── TaskManagement.Tests/        # Unit Tests
```

**Clean Architecture Pattern:**
- **API Layer**: Controllers, DTOs, HTTP concerns
- **Core Layer**: Entities, interfaces, business logic
- **Infrastructure Layer**: DbContext, repositories, external services
- **Tests Layer**: Unit tests for all layers

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure SQL Database](https://azure.microsoft.com/services/sql-database/)
- [Azure Storage Account](https://azure.microsoft.com/services/storage/) (optional for images)
- [SQL Server Management Studio](https://aka.ms/ssmsfullsetup) or Azure Data Studio
- Git

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/TaskManagementApp.git
cd TaskManagementApp
```

### 2. Azure SQL Database Setup

#### Option A: Azure Portal
1. Go to [Azure Portal](https://portal.azure.com)
2. Create a new SQL Database
3. Note your server name, database name, username, and password

#### Option B: Azure CLI
```bash
# Login to Azure
az login

# Create resource group
az group create --name TaskManagementRG --location eastus

# Create SQL Server
az sql server create \
  --name taskmanagement-server \
  --resource-group TaskManagementRG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourPassword123!

# Create database
az sql db create \
  --resource-group TaskManagementRG \
  --server taskmanagement-server \
  --name TaskManagementDB \
  --service-objective Basic
```

### 3. Run Database Schema

1. Open SQL Server Management Studio or Azure Data Studio
2. Connect to your Azure SQL Database
3. Run the `DatabaseSchema.sql` script (from artifacts above)

### 4. Azure Blob Storage Setup (Optional)

```bash
# Create storage account
az storage account create \
  --name taskmanagementstore \
  --resource-group TaskManagementRG \
  --location eastus \
  --sku Standard_LRS

# Get connection string
az storage account show-connection-string \
  --name taskmanagementstore \
  --resource-group TaskManagementRG
```

### 5. Configure Connection Strings

Update `TaskManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TaskManagementDB;Persist Security Info=False;User ID=sqladmin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=taskmanagementstore;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net"
  }
}
```

**Important:** Never commit real connection strings to GitHub. Use User Secrets for development:

```bash
cd TaskManagement.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "ConnectionStrings:BlobStorage" "YOUR_BLOB_CONNECTION_STRING"
```

### 6. Restore Dependencies

```bash
dotnet restore
```

### 7. Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

Expected output: All tests pass ✅

### 8. Run the Application

```bash
cd TaskManagement.API
dotnet run
```

The API will start at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`

### 9. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:7001/swagger
```

## API Endpoints

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks/column/{columnId}` - Get tasks in a column
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `PUT /api/tasks/{id}/move` - Move task to different column
- `PUT /api/tasks/{id}/favorite` - Toggle task favorite status

### Columns
- `GET /api/columns` - Get all columns with tasks
- `GET /api/columns/{id}` - Get column by ID
- `POST /api/columns` - Create new column

### Images
- `GET /api/tasks/{taskId}/images` - Get task images
- `POST /api/tasks/{taskId}/images` - Upload image
- `DELETE /api/tasks/{taskId}/images/{imageId}` - Delete image

## Example API Calls

### Create a Task
```bash
curl -X POST "https://localhost:7001/api/tasks" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Implement Feature X",
    "description": "Add new functionality",
    "deadline": "2025-11-15T00:00:00Z",
    "columnId": 1
  }'
```

### Move Task Between Columns
```bash
curl -X PUT "https://localhost:7001/api/tasks/1/move" \
  -H "Content-Type: application/json" \
  -d '{
    "newColumnId": 2,
    "newOrder": 1
  }'
```

### Upload Image
```bash
curl -X POST "https://localhost:7001/api/tasks/1/images" \
  -F "file=@screenshot.png"
```

## Database Schema

**Tables:**
- `Columns` - Work state columns (To Do, In Progress, Done)
- `Tasks` - Task items with details
- `TaskImages` - Image attachments for tasks

**Key Features:**
- Foreign key relationships with cascade delete
- Indexes on frequently queried columns
- UTC timestamps for all dates
- Support for favorites and custom ordering

## Testing Strategy

**Unit Tests Cover:**
- ✅ Repository operations (CRUD)
- ✅ Service layer (image handling)
- ✅ Controller actions and responses
- ✅ Edge cases and error handling
- ✅ Business logic (sorting, favorites)

**Test Technologies:**
- xUnit for test framework
- Moq for mocking
- FluentAssertions for readable assertions
- In-Memory Database for integration tests

## Key Design Decisions

1. **Clean Architecture** - Separation of concerns with layers
2. **Repository Pattern** - Abstraction over data access
3. **DTOs** - Separate API models from domain entities
4. **Async/Await** - All database operations are asynchronous
5. **Dependency Injection** - Loose coupling and testability
6. **UTC Timestamps** - Consistent time handling across time zones

## Sorting Logic

Tasks are sorted with the following priority:
1. **Favorites first** - `IsFavorite = true` tasks appear at the top
2. **Alphabetical** - Within each group, sorted by name (A-Z)

This ensures favorited tasks are always visible while maintaining organization.

## Future Enhancements

- User authentication and authorization
- Task assignments to users
- Comments on tasks
- Task history and audit log
- Real-time updates with SignalR
- Email notifications for deadlines
- Task dependencies and subtasks

## Troubleshooting

### Cannot connect to Azure SQL
- Check firewall rules in Azure Portal
- Add your IP address to allowed IPs
- Verify connection string format

### Tests failing
- Ensure all NuGet packages are restored
- Check .NET 8 SDK is installed
- Clean and rebuild solution

### Swagger not loading
- Check the application is running on correct port
- Verify `UseSwagger()` is in Program.cs
- Try accessing `/swagger/index.html` directly

## License

MIT License - Feel free to use this project for learning and development.

## Author

Built as part of .NET Core Developer Assessment

## Contact

For questions or issues, please open an issue on GitHub.
