using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services;
using BrewLog.Api.Middleware;
using BrewLog.Api.Converters;
using BrewLog.Api.Models;
using BrewLog.Api.Filters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure custom enum converters for string serialization
        options.JsonSerializerOptions.Converters.Add(new StringEnumConverter<RoastLevel>());
        options.JsonSerializerOptions.Converters.Add(new StringEnumConverter<BrewMethod>());
        options.JsonSerializerOptions.Converters.Add(new StringEnumConverter<EquipmentType>());
        
        // Configure property naming policy (camelCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        
        // Allow trailing commas in JSON
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        
        // Handle null values appropriately
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

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
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "BrewLog API",
        Version = "v1",
        Description = "A comprehensive coffee brewing tracking API for managing coffee beans, brewing equipment, grind settings, and brew sessions",
        Contact = new()
        {
            Name = "BrewLog API Support",
            Email = "support@brewlog.com"
        },
        License = new()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML documentation comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configure enum serialization to show string values
    c.SchemaFilter<EnumSchemaFilter>();
    c.ParameterFilter<EnumParameterFilter>();
    
    c.UseAllOfToExtendReferenceSchemas();
    c.SupportNonNullableReferenceTypes();
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
// Add global exception handling middleware first
app.UseMiddleware<GlobalExceptionMiddleware>();

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

// Make Program class accessible for testing
public partial class Program { }
