using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using JwtAuthenticationManager;

var builder = WebApplication.CreateBuilder(args);

// Ocelot configuration file added.
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);


// Jwt ile Authentication yapıladırmasını uygulamak için yazdığımız Custom Extension Method' ı çağıralım;
builder.Services.AddCustomJwtAuthentication();

var app = builder.Build();

await app.UseOcelot();

// Authentication yapılandırmasını uyarlamak için;
app.UseAuthentication();
app.UseAuthorization();

app.Run();
