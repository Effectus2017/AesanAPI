using System.Globalization;
using Api.Data;
using Api.Interfaces;
using Api.Mapper;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile(
    $"appsettings.Development.json",
    optional: true,
    reloadOnChange: true
);

builder.Services.Configure<ApplicationSettings>(
    builder.Configuration.GetSection(nameof(ApplicationSettings))
);

// Configuración de servicios
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    },
    ServiceLifetime.Scoped
);

builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services
    .AddIdentity<User, Role>(config =>
    {
        config.Password.RequiredLength = 6;
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<ApplicationSettings>();

// Agregar repositorios a la inyección de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGeoRepository, GeoRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IAgencyStatusRepository, AgencyStatusRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IMealTypeRepository, MealTypeRepository>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IFoodAuthorityRepository, FoodAuthorityRepository>();
builder.Services.AddScoped<IOperatingPeriodRepository, OperatingPeriodRepository>();
builder.Services.AddScoped<IOperatingPolicyRepository, OperatingPolicyRepository>();
builder.Services.AddScoped<IOrganizationTypeRepository, OrganizationTypeRepository>();
builder.Services.AddScoped<IEducationLevelRepository, EducationLevelRepository>();

// Registrar servicios Lazy
builder.Services.AddScoped<Lazy<IUserRepository>>(sp => new Lazy<IUserRepository>(() => sp.GetRequiredService<IUserRepository>()));
builder.Services.AddScoped<Lazy<IAgencyRepository>>(sp => new Lazy<IAgencyRepository>(() => sp.GetRequiredService<IAgencyRepository>()));

// Registrar AgencyUsersRepository después de los servicios Lazy
builder.Services.AddScoped<IAgencyUsersRepository, AgencyUsersRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Agregar UnitOfWork

// Agregar controladores a la inyección de dependencias
builder.Services.AddControllers();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo { Title = "AESAN API", Version = "v1" }
    );
});

// Configuración de Dapper
builder.Services.AddScoped<DapperContext>();

// Registro de SendGrid
builder.Services.AddSingleton<ISendGridClient>(
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"])
);

// Configuración de AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowDevOrigin",
        builder =>
            builder
                .WithOrigins(
                    "http://localhost:4202",
                    "https://localhost:4202",
                    "http://localhost:5002",
                    "https://localhost:5002",
                    "https://nutre-dev.local:4202",
                    "https://aesanweb-dev.azurewebsites.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true) // Solo para desarrollo
    );
    options.AddPolicy(
        "AllowProdOrigin",
        builder =>
            builder
                .WithOrigins(
                    "https://aesanweb-fwbfa9hshaaybnbf.canadacentral-01.azurewebsites.net",
                    "https://aesanweb-dev.azurewebsites.net",
                    "https://aesanapi.azurewebsites.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin =>
                {
                    // Log the origin for debugging
                    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Requested Origin: {Origin}", origin);
                    return origin.EndsWith("azurewebsites.net");
                })
    );
});

// Configurar la cultura invariante
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

builder.Services.AddMemoryCache();

// Agregar Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = false; // Deshabilitar el muestreo adaptativo si quieres todos los logs
});

// Configurar el TelemetryConfiguration para enriquecer los logs
builder.Services.Configure<TelemetryConfiguration>((config) =>
{
    config.TelemetryInitializers.Add(new EnvironmentTelemetryInitializer(builder.Environment));
});

builder.Services.AddScoped<ILoggingService, LoggingService>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddApplicationInsights();
    logging.SetMinimumLevel(LogLevel.Warning);
});

var app = builder.Build();

// Configuración de middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowDevOrigin");
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            logger.LogError(
                exception,
                "Error no manejado: {Message}. Path: {Path}",
                exception?.Message,
                exceptionHandlerPathFeature?.Path
            );

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Se produjo un error interno en el servidor",
                path = exceptionHandlerPathFeature?.Path,
                detail = app.Environment.IsDevelopment() ? exception?.ToString() : null
            });
        });
    });
    app.UseHsts();
    app.UseCors("AllowProdOrigin");
    app.UseSwaggerUI();
}

// Configuración global
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
    c.RoutePrefix = string.Empty; // Hacer que Swagger esté disponible en la raíz
});

var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (Directory.Exists(uploadsPath))
{
    app.UseFileServer(enableDirectoryBrowsing: true)
        .UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = new PathString("/uploads")
            }
        )
        .UseDirectoryBrowser(
            new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = new PathString("/uploads")
            }
        );
}
else
{
    Directory.CreateDirectory(uploadsPath);
}

// Agregar middleware personalizado para logging de solicitudes
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    logger.LogInformation(
        "Request: {Method} {Scheme} {Host}{Path} {QueryString}",
        context.Request.Method,
        context.Request.Scheme,
        context.Request.Host,
        context.Request.Path,
        context.Request.QueryString
    );

    logger.LogInformation(
        "Headers: {@Headers}",
        context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
    );

    // Capturar el cuerpo de la respuesta
    var originalBodyStream = context.Response.Body;
    using var memoryStream = new MemoryStream();
    context.Response.Body = memoryStream;

    try
    {
        await next.Invoke();

        logger.LogInformation(
            "Response: Status: {StatusCode}, Headers: {@Headers}",
            context.Response.StatusCode,
            context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        );

        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalBodyStream);
    }
    finally
    {
        context.Response.Body = originalBodyStream;
    }
});

app.MapControllers();

app.Run();
