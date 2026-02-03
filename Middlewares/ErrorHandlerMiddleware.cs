using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Aplicacion.Excepciones;

namespace API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error no controlado");
                var statusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BusinessConflictException => StatusCodes.Status409Conflict,
                    DomainException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = statusCode switch
                    {
                        404 => "Recurso no encontrado",
                        409 => "Conflicto de negocio",
                        400 => "Solicitud inválida",
                        _ => "Error interno del servidor"
                    },
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
            }
        }
    }
}
