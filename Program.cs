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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true
);

builder.Services.Configure<ApplicationSettings>(
    builder.Configuration.GetSection(nameof(ApplicationSettings))
);

// Configuración de servicios
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient
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
builder.Services.AddScoped<IAgencyUserAssignmentRepository, AgencyUserAssignmentRepository>();
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
                .WithOrigins("http://localhost:4202")
                .WithOrigins("https://nutre-dev.local:4202")
                .WithOrigins("https://aesanweb-dev.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
    options.AddPolicy(
        "AllowProdOrigin",
        builder =>
            builder
                .WithOrigins("http://localhost:4202")
                .WithOrigins("https://aesanweb-fwbfa9hshaaybnbf.canadacentral-01.azurewebsites.net")
                .WithOrigins("https://aesanweb-dev.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
});

// Configurar la cultura invariante
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configuración de middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage().UseCors("AllowDevOrigin").UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection().UseCors("AllowProdOrigin").UseSwaggerUI();
}

app.UseHttpsRedirection();
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

app.MapControllers();

app.Run();
