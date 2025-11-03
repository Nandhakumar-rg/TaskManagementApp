-- Task Management Database Schema for Azure SQL

-- Drop tables if they exist (for clean setup)
IF OBJECT_ID('TaskImages', 'U') IS NOT NULL DROP TABLE TaskImages;
IF OBJECT_ID('Tasks', 'U') IS NOT NULL DROP TABLE Tasks;
IF OBJECT_ID('Columns', 'U') IS NOT NULL DROP TABLE [Columns];

-- Columns Table (represents work states like ToDo, In Progress, Done)
CREATE TABLE [Columns] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    [Order] INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Tasks Table
CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    Deadline DATETIME2 NULL,
    ColumnId INT NOT NULL,
    IsFavorite BIT NOT NULL DEFAULT 0,
    [Order] INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Tasks_Columns FOREIGN KEY (ColumnId) REFERENCES [Columns](Id) ON DELETE CASCADE
);

-- TaskImages Table (for image attachments)
CREATE TABLE TaskImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    BlobName NVARCHAR(200) NOT NULL,
    FileName NVARCHAR(200) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    UploadedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_TaskImages_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE
);

-- Create Indexes for better performance
CREATE INDEX IX_Tasks_ColumnId ON Tasks(ColumnId);
CREATE INDEX IX_Tasks_IsFavorite ON Tasks(IsFavorite);
CREATE INDEX IX_Tasks_Name ON Tasks(Name);
CREATE INDEX IX_TaskImages_TaskId ON TaskImages(TaskId);

-- Insert default columns
INSERT INTO [Columns] (Name, [Order]) VALUES 
('To Do', 1),
('In Progress', 2),
('Done', 3);

-- Sample data for testing
INSERT INTO Tasks (Name, [Description], Deadline, ColumnId, IsFavorite, [Order])
VALUES 
('Setup Project', 'Initialize .NET Core project with Azure SQL', DATEADD(day, 7, GETUTCDATE()), 1, 1, 1),
('Design Database', 'Create database schema with proper relationships', DATEADD(day, 5, GETUTCDATE()), 2, 0, 1),
('Implement API', 'Build REST API with CRUD operations', DATEADD(day, 10, GETUTCDATE()), 1, 0, 2);

GO