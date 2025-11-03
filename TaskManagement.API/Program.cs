using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("[startup-trace] Builder created");


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "A simple task management API built with .NET Core and Azure SQL"
    });
});

// Configure DbContext
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register repositories and services
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IColumnRepository, ColumnRepository>();
builder.Services.AddScoped<IImageService>(provider =>
{
    var context = provider.GetRequiredService<TaskManagementDbContext>();
    var blobConnectionString = builder.Configuration.GetConnectionString("BlobStorage");
    return new ImageService(context, blobConnectionString);
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

Console.WriteLine("[startup-trace] Building app...");
var app = builder.Build();
Console.WriteLine("[startup-trace] App built");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("[startup-trace] About to run");
app.Run();