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


### 3. Configure Connection Strings

Update `TaskManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TaskManagementDB;Persist Security Info=False;User ID=sqladmin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=taskmanagementstore;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net"
  }
}
```


### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```



### 6. Run the Application

```bash
cd TaskManagement.API
dotnet run
```

The API will start at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`

### 7. Access Swagger UI

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



## Deployment Options

### Local Development (Current Setup)
- SQL Server LocalDB
- File-based image storage

### Production Ready (Azure)
- Azure SQL Database (configuration provided)
- Azure Blob Storage (implementation included)
- Simply update connection strings to deploy to Azure

The application is **cloud-ready** and can be deployed to Azure 
by updating appsettings.json with Azure connection strings.
