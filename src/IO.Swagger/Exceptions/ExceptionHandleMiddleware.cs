using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using IO.Swagger.Models;

namespace IO.Swagger.Exceptions
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// The function warps each Api and handles the exceptions
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(ex, httpContext);
            }
        }

        private async Task HandleException(Exception ex, HttpContext httpContext)
        {
            if (ex is InvalidOperationException)
            {
                httpContext.Response.StatusCode = 400;            
                await httpContext.Response.WriteAsJsonAsync("Bad request");
            }
            else if (ex is ArgumentException)
            {
                await httpContext.Response.WriteAsync("Invalid argument");
            }
            else if (ex is DivideByZeroException)
            {
                await httpContext.Response.WriteAsync("can't divide by zero");
            }
            else
            {
                await httpContext.Response.WriteAsync("Unknown error");
            }
        }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline
    /// </summary>
    public static class ExceptionHandleMiddlewareExtensions
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the exception handle middleware function.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }
}
