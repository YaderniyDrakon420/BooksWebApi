using Controllers.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Controllers.Middleware;

public class BookMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<Book> _books;

    public BookMiddleware(RequestDelegate next, List<Book> books)
    {
        _next = next;
        _books = books;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"{context.Request.Method} {context.Request.Path} — {DateTime.Now:HH:mm:ss}");

        if (context.Request.Path.StartsWithSegments("/api/books") &&
            context.Request.Method == HttpMethods.Get &&
            context.Request.RouteValues.TryGetValue("id", out var idObj) &&
            int.TryParse(idObj?.ToString(), out int id))
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Book not found");
                return;
            }

            context.Items["book"] = book;
        }

        await _next(context);
    }
}

public static class BookMiddlewareExtensions
{
    public static IApplicationBuilder UseBookMiddleware(this IApplicationBuilder builder, List<Book> books)
    {
        return builder.UseMiddleware<BookMiddleware>(books);
    }
}