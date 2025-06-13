using System.Globalization;
using Api.Data;
using Api.Extensions;
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
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, true);

// Configurar ApplicationSettings usando IOptions pattern
var appSettingsSection = builder.Configuration.GetSection(nameof(ApplicationSettings));
builder.Services.Configure<ApplicationSettings>(appSettingsSection);

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

// Configurar HttpContextAccessor
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddScoped<IHouseholdRepository, HouseholdRepository>();
builder.Services.AddScoped<IKitchenTypeRepository, KitchenTypeRepository>();
builder.Services.AddScoped<IHouseholdMemberIncomeRepository, HouseholdMemberIncomeRepository>();
builder.Services.AddScoped<IOptionSelectionRepository, OptionSelectionRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IGroupTypeRepository, GroupTypeRepository>();
builder.Services.AddScoped<ISponsorTypeRepository, SponsorTypeRepository>();
builder.Services.AddScoped<IDeliveryTypeRepository, DeliveryTypeRepository>();
builder.Services.AddScoped<ICenterTypeRepository, CenterTypeRepository>();
builder.Services.AddScoped<IHouseholdMemberRepository, HouseholdMemberRepository>();
// Registrar servicios Lazy
builder.Services.AddScoped<Lazy<IUserRepository>>(sp => new Lazy<IUserRepository>(() => sp.GetRequiredService<IUserRepository>()));
builder.Services.AddScoped<Lazy<IAgencyRepository>>(sp => new Lazy<IAgencyRepository>(() => sp.GetRequiredService<IAgencyRepository>()));

// Registrar AgencyUsersRepository después de los servicios Lazy
builder.Services.AddScoped<IAgencyUsersRepository, AgencyUsersRepository>();
builder.Services.AddScoped<IAgencyFilesRepository, AgencyFilesRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

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
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

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
                    "https://aesanweb-dev.azurewebsites.net",
                    "https://aesanapi.azurewebsites.net",
                    "https://webaes.azurewebsites.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => origin.EndsWith("azurewebsites.net"))
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
    options.EnableAdaptiveSampling = true; // Deshabilitar el muestreo adaptativo si quieres todos los logs
});

// Configurar el TelemetryConfiguration para enriquecer los logs
builder.Services.Configure<TelemetryConfiguration>((config) =>
{
    config.TelemetryInitializers.Add(new EnvironmentTelemetryInitializer(builder.Environment));
});

// Configuración de ELMAH con SQL Server
builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Usa la misma conexión que la aplicación
    options.Path = "/elmah"; // Ruta para acceder al dashboard de ELMAH
    options.OnPermissionCheck = context => context.User.Identity.IsAuthenticated; // Solo usuarios autenticados pueden ver el dashboard
});

builder.Services.AddScoped<ILoggingService, LoggingService>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddApplicationInsights();
    logging.SetMinimumLevel(LogLevel.Warning);
});

// --- INICIO: Configuración de políticas de permisos granulares (deshabilitado por ahora) ---
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("Permission:UserView", policy => policy.RequireClaim("permission", "UserView"));
//     options.AddPolicy("Permission:UserCreate", policy => policy.RequireClaim("permission", "UserCreate"));
//     options.AddPolicy("Permission:UserEdit", policy => policy.RequireClaim("permission", "UserEdit"));
//     options.AddPolicy("Permission:UserDelete", policy => policy.RequireClaim("permission", "UserDelete"));
//     options.AddPolicy("Permission:UserAssignRole", policy => policy.RequireClaim("permission", "UserAssignRole"));
//     options.AddPolicy("Permission:UserAssignAgency", policy => policy.RequireClaim("permission", "UserAssignAgency"));
//     options.AddPolicy("Permission:RoleView", policy => policy.RequireClaim("permission", "RoleView"));
//     options.AddPolicy("Permission:RoleCreate", policy => policy.RequireClaim("permission", "RoleCreate"));
//     options.AddPolicy("Permission:RoleEdit", policy => policy.RequireClaim("permission", "RoleEdit"));
//     options.AddPolicy("Permission:RoleDelete", policy => policy.RequireClaim("permission", "RoleDelete"));
//     options.AddPolicy("Permission:RoleAssignPermission", policy => policy.RequireClaim("permission", "RoleAssignPermission"));
//     options.AddPolicy("Permission:AgencyView", policy => policy.RequireClaim("permission", "AgencyView"));
//     options.AddPolicy("Permission:AgencyCreate", policy => policy.RequireClaim("permission", "AgencyCreate"));
//     options.AddPolicy("Permission:AgencyEdit", policy => policy.RequireClaim("permission", "AgencyEdit"));
//     options.AddPolicy("Permission:AgencyDelete", policy => policy.RequireClaim("permission", "AgencyDelete"));
//     options.AddPolicy("Permission:AgencyApprove", policy => policy.RequireClaim("permission", "AgencyApprove"));
//     options.AddPolicy("Permission:AgencyAssignUser", policy => policy.RequireClaim("permission", "AgencyAssignUser"));
//     options.AddPolicy("Permission:ProgramView", policy => policy.RequireClaim("permission", "ProgramView"));
//     options.AddPolicy("Permission:ProgramCreate", policy => policy.RequireClaim("permission", "ProgramCreate"));
//     options.AddPolicy("Permission:ProgramEdit", policy => policy.RequireClaim("permission", "ProgramEdit"));
//     options.AddPolicy("Permission:ProgramDelete", policy => policy.RequireClaim("permission", "ProgramDelete"));
//     options.AddPolicy("Permission:ProgramAssignAgency", policy => policy.RequireClaim("permission", "ProgramAssignAgency"));
//     options.AddPolicy("Permission:DocumentView", policy => policy.RequireClaim("permission", "DocumentView"));
//     options.AddPolicy("Permission:DocumentUpload", policy => policy.RequireClaim("permission", "DocumentUpload"));
//     options.AddPolicy("Permission:DocumentDelete", policy => policy.RequireClaim("permission", "DocumentDelete"));
//     options.AddPolicy("Permission:DocumentDownload", policy => policy.RequireClaim("permission", "DocumentDownload"));
//     options.AddPolicy("Permission:SchoolView", policy => policy.RequireClaim("permission", "SchoolView"));
//     options.AddPolicy("Permission:SchoolCreate", policy => policy.RequireClaim("permission", "SchoolCreate"));
//     options.AddPolicy("Permission:SchoolEdit", policy => policy.RequireClaim("permission", "SchoolEdit"));
//     options.AddPolicy("Permission:SchoolDelete", policy => policy.RequireClaim("permission", "SchoolDelete"));
//     options.AddPolicy("Permission:ReportView", policy => policy.RequireClaim("permission", "ReportView"));
//     options.AddPolicy("Permission:DashboardView", policy => policy.RequireClaim("permission", "DashboardView"));
//     options.AddPolicy("Permission:CatalogView", policy => policy.RequireClaim("permission", "CatalogView"));
//     options.AddPolicy("Permission:CatalogEdit", policy => policy.RequireClaim("permission", "CatalogEdit"));
//     options.AddPolicy("Permission:AuditView", policy => policy.RequireClaim("permission", "AuditView"));
// });
// --- FIN: Configuración de políticas de permisos granulares ---

var app = builder.Build();

// Middleware para logging de CORS
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var origin = context.Request.Headers["Origin"].ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        logger.LogInformation(
            "CORS Request - Origin: {Origin}, Path: {Path}, Method: {Method}",
            origin,
            context.Request.Path,
            context.Request.Method
        );
    }
    await next();
});

// Configuración de middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

            // Registrar en ELMAH
            await context.RaiseError(exception);

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
}

// CORS debe ir antes de routing y después de los middleware de error
app.UseCors(app.Environment.IsDevelopment() ? "AllowDevOrigin" : "AllowProdOrigin");

// Habilitar ELMAH
app.UseElmah();

app.UseHttpsRedirection();

// Configuración global
app.UseRouting();

// Asegurar que la carpeta uploads existe
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

// Habilitar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AESAN API V1");
        c.RoutePrefix = string.Empty;
    });
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
