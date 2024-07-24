using DAL.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL.Middleware
{


    public class CheckIfManager
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public CheckIfManager(RequestDelegate next, IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method.ToUpper() == "DELETE")
            {

                var path = context.Request.Path.ToString();
                var routeData = context.GetRouteData();
                int userId = GetUserIdFromJwt(context);

                if (context.Request.Path.StartsWithSegments("/Event"))
                {
                    if (routeData.Values.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue.ToString(), out int eventId))
                    {
                        // חילוץ ה-UserId מה-JWT

                        if (userId > 0)
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var _event = scope.ServiceProvider.GetRequiredService<IEvent>();

                                // קבלת פרטי האירוע מהשירות
                                var eventDetails = await _event.getEventById(eventId);

                                if (eventDetails != null && eventDetails.Owner == userId)
                                {
                                    // המשתמש הוא הבעלים של האירוע, המשך לבקשה הבאה בצנרת
                                    await _next(context);
                                    return;
                                }
                                else
                                {
                                    // אם המשתמש לא מזוהה או לא הבעלים של האירוע
                                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                    await context.Response.WriteAsync("You are not authorized to delete this event.");
                                    return;
                                }
                            }
                        }

                    }
                }

                if (context.Request.Path.StartsWithSegments("/Group"))
                {

                    // המידלוור צריך להיות ממוקם לאחר UseRouting()

                    if (routeData.Values.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue.ToString(), out int eventId))
                    {
                        // חילוץ ה-UserId מה-JWT
                        int userId = GetUserIdFromJwt(context);

                        if (userId > 0)
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var _event = scope.ServiceProvider.GetRequiredService<IEvent>();

                                // קבלת פרטי האירוע מהשירות
                                var eventDetails = await _event.getEventById(eventId);

                                if (eventDetails != null && eventDetails.Owner == userId)
                                {
                                    // המשתמש הוא הבעלים של האירוע, המשך לבקשה הבאה בצנרת
                                    await _next(context);
                                    return;
                                }
                                else
                                {
                                    // אם המשתמש לא מזוהה או לא הבעלים של האירוע
                                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                    await context.Response.WriteAsync("You are not authorized to delete this event.");
                                    return;
                                }
                            }
                        }

                    }
                }

            }
            await _next(context);
            return;
        }

        private int GetUserIdFromJwt(HttpContext context)
        {
            var jwt = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jsonToken = handler.ReadToken(jwt) as JwtSecurityToken;
                if (jsonToken != null)
                {
                    // חילוץ ה-User ID מתוך ה-Claims של ה-JWT
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "id");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch (Exception ex)
            {
                // לוג החריגות
                Console.WriteLine($"Error extracting user ID from JWT: {ex.Message}");
            }

            return -1;
        }
    }
}
