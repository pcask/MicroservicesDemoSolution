using Microsoft.EntityFrameworkCore;
using ProductWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Database Context Dependency Injection

string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
string dbName = Environment.GetEnvironmentVariable("DB_NAME");
string dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

string connectionString = $"server={dbHost}; port=3306; database={dbName}; user=root; password={dbPassword}";
builder.Services.AddDbContext<ProductDbContext>(opt => opt.UseMySQL(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
