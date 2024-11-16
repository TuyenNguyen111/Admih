using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace FinalProject.Middleware
{
    public class PageAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public PageAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var requestPath = context.Request.Path.Value;

            if (requestPath == "/")
            {
                await _next(context);
                return;
            }

            var cleanedRequestPath = RemoveIdFromPath(requestPath);
            var userClaims = context.User.Claims;

            if (!userClaims.Any(c => c.Type == "PageAccess" && c.Value == cleanedRequestPath))
            {
                //await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Response.Redirect("/Home/AccessDenied");
                return;
            }

            await _next(context);
        }

        private string RemoveIdFromPath(string path)
        {
            var segments = path.Split('/');
            return segments.Length >= 3 && int.TryParse(segments.Last(), out _)
                ? string.Join('/', segments.Take(segments.Length - 1))
                : path;
        }


    }
}
