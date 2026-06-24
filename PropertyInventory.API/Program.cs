using Microsoft.EntityFrameworkCore;
using PropertyInventory.API.Middleware;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Application.Services;
using PropertyInventory.Infrastructure.Data;
using PropertyInventory.Infrastructure.Data.Seed;
using PropertyInventory.Infrastructure.Repositories;
using PropertyInventory.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Persistence ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// --- Repositories ---
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// --- Services ---
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();
builder.Services.AddMemoryCache();

// --- Error handling (RFC 7807) ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- MVC / Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.MapControllers();

// --- Apply migrations + seed on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}

app.Run();

// Exposed so WebApplicationFactory-based integration tests can reference the entry point.
public partial class Program { }
