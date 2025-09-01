using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<BrewLogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ??
                     "Data Source=brewlog.db"));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICoffeeBeanRepository, CoffeeBeanRepository>();
builder.Services.AddScoped<IGrindSettingRepository, GrindSettingRepository>();
builder.Services.AddScoped<IBrewingEquipmentRepository, BrewingEquipmentRepository>();
builder.Services.AddScoped<IBrewSessionRepository, BrewSessionRepository>();

// Register Services
builder.Services.AddScoped<ICoffeeBeanService, CoffeeBeanService>();
builder.Services.AddScoped<IGrindSettingService, GrindSettingService>();
builder.Services.AddScoped<IBrewingEquipmentService, BrewingEquipmentService>();
builder.Services.AddScoped<IBrewSessionService, BrewSessionService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "BrewLog API",
        Version = "v1",
        Description = "A comprehensive coffee brewing tracking API"
    });
});

// Add CORS for frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BrewLog API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BrewLogDbContext>();
    context.Database.Migrate();
}

app.Run();
