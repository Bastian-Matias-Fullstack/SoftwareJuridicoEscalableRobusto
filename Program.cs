using API.Middlewares;
using Aplicacion.Casos;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Aplicacion.Servicios.Auth;
using Aplicacion.Servicios.Casos;
using Aplicacion.Validaciones;
using Infraestructura.Persistencia;
using Infraestructura.Repositorios;
using Infraestructura.Servicios;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;

//Configuración de Servicios (DI)
var builder = WebApplication.CreateBuilder(args);

//aqui permitimos 
//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//Conexion a la base de datos 
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICasoRepository, CasoRepository>();
builder.Services.AddScoped<ListarCasosService>();
builder.Services.AddScoped<ActualizarCasoService>();
builder.Services.AddScoped<FormateadorNombreService>();
builder.Services.AddScoped<CrearCasoService>();
builder.Services.AddScoped<CerrarCasoService>();
builder.Services.AddScoped<EliminarCasoService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IRolRepositorio, RolRepositorio>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Aplicacion.Usuarios.Handlers.CrearUsuarioCommandHandler>());
builder.Services.AddHttpContextAccessor();
//🔹 Validaciones (FluentValidation)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CrearCasoRequestValidator>();
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value is not null && e.Value.Errors.Count > 0)
            .Select(e => new
            {
                campo = e.Key,
                errores = e.Value!.Errors.Select(x =>
                    string.IsNullOrWhiteSpace(x.ErrorMessage)
                        ? "Valor inválido."
                        : x.ErrorMessage
                ).ToList()
            })
            .ToList();

        var problemDetails = new ProblemDetails
        {
            Title = "Solicitud inválida",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Uno o más parámetros no cumplen el formato esperado.",
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions["errors"] = errors;

        return new BadRequestObjectResult(problemDetails);
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); //  Esto es clave

    c.SwaggerDoc("v1", new OpenApiInfo
    { 
        Title = "API Jurídica",
        Version = "v1",
        Description = "Documentación oficial de la API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ejemplo: Bearer {tu_token}"
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
    c.UseInlineDefinitionsForEnums(); //Esto activa los enums como dropdown en Swagger

});
// 1. CORS
// 1) CORS (por configuración)
var corsOrigins = builder.Configuration
    .GetSection("Cors:Origins")
    .Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        if (corsOrigins.Length > 0)
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        // Si está vacío, no abrimos CORS (y en local con wwwroot no lo necesitas)
    });
});
// Autenticación con JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
    "JWT Key no configurada. Configura Jwt:Key (Development) o la variable de entorno Jwt__Key (Production).");
}
    builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero

        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
//builder.Services.AddEndpointsApiExplorer(); // Necesario para Swagger
var app = builder.Build();
// ✅ Swagger controlado por configuración
var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Jurídica v1");
        c.DocumentTitle = "Documentación API Jurídica";
        c.RoutePrefix = "swagger";
    });
}
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("PermitirFrontend"); // ESTO ACTIVA CORS
app.UseAuthentication(); // JWT primero
app.UseAuthorization();
app.MapControllers();
app.Run();
