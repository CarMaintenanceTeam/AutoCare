
using AutoCare.Infrastructure;
using AutoCare.Application;
using AutoCare.Application.Common.Middleware;
using Microsoft.OpenApi.Models;
using Serilog;

// ============ CONFIGURE SERILOG ============
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(
    path: "logs/AutoCareLog-.log",
    rollingInterval: RollingInterval.Day,
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    ).CreateLogger();

try
{

    Log.Information("starting AutoCare Platform API...");
    // Create Web Application Builder
    var builder = WebApplication.CreateBuilder(args);

    // ============ ADD LOGGING ============

    // Replace default logging with Serilog
    builder.Host.UseSerilog();

    // ============ ADD SERVICES ============

    // Application Layer (MediatR, FluentValidation, Mapster)
    builder.Services.AddApplication();

    // Infrastructure Layer (Database, Authentication, Services)
    builder.Services.AddInfrastructure(builder.Configuration);

    // Controllers
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

    // Swagger/OpenAPI

    //API Explorer
    builder.Services.AddEndpointsApiExplorer();

    // ============ SWAGGER CONFIGURATION ============
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AutoCare Platform API",
            Version = "v1",
            Description = "API for managing car maintenance bookings and services",
            Contact = new OpenApiContact
            {
                Name = "AutoCare Support",
                Email = "belalfayez@outlook.com"
            }
        });

        // Add JWT Authentication to Swagger UI
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. " +
                             "Enter 'Bearer' [space] and then your token in the text input below. " +
                             "Example: 'Bearer eyJhbGci...'"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
        });

        // Include XML comments (assuming XML comments file is generated)
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (!File.Exists(xmlPath))
        {
            Log.Warning("XML comments file not found: {XmlPath}", xmlPath);
        }
        else
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",  // React CRA
                    "http://localhost:5173",  // Vite
                    "http://localhost:5174"   // Vite alternative port
                  )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("Token-Expired"); // Expose custom headers
        });
    });

    // ============ BUILD APP ============

    var app = builder.Build();

    // ============ CONFIGURE MIDDLEWARE PIPELINE ============
    // Global Exception Handler (should be first)
    app.UseGlobalExceptionHandler();

    // Serilog Request Logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    // Development Tools
    if (app.Environment.IsDevelopment())
    {
        // Detailed Exception Page
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // Global Exception Handler
        app.UseExceptionHandler("/error");
        // HSTS for production
        app.UseHsts();
    }



    // Swagger (Development & Staging)
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoCare API v1");
            options.RoutePrefix = string.Empty; // Serve Swagger UI at app root
            options.DocumentTitle = "AutoCare API Documentation";
        });
    }

    // Security Headers
    app.UseHttpsRedirection();

    // CORS (must be before Authentication)
    app.UseCors("AllowReactApp");

    // Authentication & Authorization (ORDER MATTERS!)
    app.UseAuthentication();
    app.UseAuthorization();

    // Map Controllers
    app.MapControllers();

    // Health Check Endpoint
    app.MapGet("/health", () => Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow,
        version = "1.0.0"
    }))
    .WithName("HealthCheck")
    .WithOpenApi();

    // Run Application
    Log.Information("AutoCare Platform API started successfully");
    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}














