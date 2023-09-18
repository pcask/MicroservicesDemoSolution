using CustomerWebApi;
using Microsoft.EntityFrameworkCore;
using JwtAuthenticationManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

/* Database Context Dependency Injection */
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

var connectionString = $"Data source ={dbHost}; Initial Catalog={dbName}; User ID=sa; Password={dbPassword}; Encrypt=False";
//  A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.//
// Yukardaki hatayı şimdilik "Encrypt=False" ataması ile giderdim.

builder.Services.AddDbContext<CustomerDbContext>(opt => opt.UseSqlServer(connectionString));
/* ---------------------------------- */


// Jwt ile Authentication yapıladırmasını uygulamak için yazdığımız Custom Extension Method' ı çağıralım;
builder.Services.AddCustomJwtAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Authentication yapılandırmasını uyarlamak için;
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
