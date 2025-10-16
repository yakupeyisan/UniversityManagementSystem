using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using UniversityMS.Api.Middleware;
using UniversityMS.Application;
using UniversityMS.Infrastructure;
using UniversityMS.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. SERILOG CONFIGURATION =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10485760)
    .CreateLogger();

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting University Management System API...");

    // ===== 2. ADD SERVICES TO CONTAINER =====

    // Controllers
    builder.Services.AddControllers();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "University Management System API",
            Version = "v1",
            Description = "Comprehensive University Management System API with Clean Architecture",
            Contact = new OpenApiContact
            {
                Name = "Yakup Eyisan",
                Email = "yakupeyisan@gmail.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT"
            }
        });

        // JWT Authentication for Swagger
        //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //{
        //    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        //    Name = "Authorization",
        //    In = ParameterLocation.Header,
        //    Type = SecuritySchemeType.ApiKey,
        //    Scheme = "Bearer"
        //});
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["Secret"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // HttpContext Accessor
    builder.Services.AddHttpContextAccessor();

    // Application Layer
    builder.Services.AddApplication();

    // Infrastructure Layer
    builder.Services.AddInfrastructure(builder.Configuration);

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>();

    // ===== 3. BUILD APP =====
    var app = builder.Build();

    // ===== 4. CONFIGURE MIDDLEWARE PIPELINE =====

    // Global Error Handling
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Swagger - Development & Production
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "University MS API v1");
            c.RoutePrefix = "swagger";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });
    }

    // HTTPS Redirection
    app.UseHttpsRedirection();

    // CORS
    app.UseCors("AllowAll");

    // Serilog Request Logging
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Controllers
    app.MapControllers();

    // Health Checks
    app.MapHealthChecks("/health");

    // Root Endpoint
    app.MapGet("/", () => Results.Ok(new
    {
        Application = "University Management System API",
        Version = "1.0.0",
        Status = "Running",
        Timestamp = DateTime.UtcNow,
        Environment = app.Environment.EnvironmentName,
        Endpoints = new
        {
            Swagger = "/swagger",
            Health = "/health",
            Api = "/api/v1"
        }
    }))
    .WithName("Root")
    .WithTags("System");

    // ===== 5. RUN APPLICATION =====
    Log.Information("Application started successfully on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
