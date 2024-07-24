using Microsoft.AspNetCore.Http;
using System.Net;


namespace BL.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Allow the request to pass through the pipeline
            await _next(context);
            // After the request has been processed by the next middleware/component
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    context.Response.StatusCode,
                    Message = "Access denied. You do not have sufficient permissions to access this resource."
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        }
    }
}