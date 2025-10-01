using AuthApi.Interfaces;
using AuthApi.Repositorios;
using AuthApi.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AuthApi.Mappers;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------
// CONFIGURACIÓN DE BASE DE DATOS Y DEPENDENCIAS
// -----------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    // Carga la cadena de conexión desde appsettings.json
    var connectionString = builder.Configuration.GetConnectionString("Conn");

    // Configura MySQL con el proveedor Pomelo
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

    // Opciones de depuración para ver por qué falla la conexión
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

// -----------------------------------------------------------------
// REGISTRO DE REPOSITORIOS (Acceso a Datos)
// -----------------------------------------------------------------

// Registro de Usuario y Auth
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthRepository>();

// Repositorios futuros (descomentar cuando la interfaz y clase existan)
// builder.Services.AddScoped<IRolRepository, RolRepository>(); 
// builder.Services.AddScoped<IProductoRepository, ProductoRepository>(); 

// Registro de Categoría
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// -----------------------------------------------------------------
// REGISTRO DE SERVICIOS (Lógica de Negocio)
// -----------------------------------------------------------------

// Registro del Servicio de Categoría
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
// Si tienes un servicio de Producto, regístralo aquí:
// builder.Services.AddScoped<IProductoService, ProductoService>();


// -----------------------------------------------------------------
// REGISTRO DE AUTO MAPPER
// -----------------------------------------------------------------
builder.Services.AddAutoMapper(typeof(Program));


// -----------------------------------------------------------------
// CONFIGURACIÓN DE AUTHENTICATION, MVC Y SWAGGER
// -----------------------------------------------------------------
builder.Services.AddAuthentication("Bearer")
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de SwaggerGen
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutService API", Version = "v1" });

    // Definición de seguridad (JWT Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa 'Bearer' seguido de un espacio y su token JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Requisito de seguridad (aplica JWT a los endpoints)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication() debe ir antes de app.UseAuthorization()
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
