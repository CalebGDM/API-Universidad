using Microsoft.EntityFrameworkCore;
using API_Universidad.Data;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde .env
Env.Load();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Configurar conexión a Azure SQL
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AzureDB");

Console.WriteLine("=== CONNECTION STRING DEBUG ===");
Console.WriteLine($"Full connection string: {connectionString}");
Console.WriteLine("================================");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "ERROR: No se encontró el Connection String.\n" +
        "Verifica tu archivo .env"
    );
}

builder.Services.AddDbContext<UniversidadContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();