using Controllers.Middleware;
using Controllers.Models;

namespace Controllers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        var books = new List<Book>
        {
            new Book(1, "1984", "George Orwell", 1949),
            new Book(2, "The Master and Margarita", "Mikhail Bulgakov", 1967),
            new Book(3, "Crime and Punishment", "Fyodor Dostoevsky", 1866)
        };

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseBookMiddleware(books);

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
