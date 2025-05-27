using APBD12.Models;
using APBD12.Services;
using Microsoft.EntityFrameworkCore;
using DbContext = APBD12.Models.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja kontekstu bazy danych (najpierw!)
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Rejestracja serwisów
builder.Services.AddScoped<IDbService, DbService>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Konfiguracja pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();