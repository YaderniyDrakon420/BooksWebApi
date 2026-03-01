using Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private static readonly List<Book> Books = new()
    {
        new Book(1, "1984", "George Orwell", 1949),
        new Book(2, "The Master and Margarita", "Mikhail Bulgakov", 1967),
        new Book(3, "Crime and Punishment", "Fyodor Dostoevsky", 1866)
    };

    [HttpGet]
    public IActionResult GetBooks()
    {
        return Ok(Books);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetBookById(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return NotFound("Book not found");

        return Ok(book);
    }

    [HttpGet("search")]
    public IActionResult Search(string? title, string? author)
    {
        if (title == null || title.Trim() == "")
            return BadRequest("Title is required");

        string titleLower = title.ToLower();

        var result = new List<Book>();

        foreach (var book in Books)
        {
            if (!book.Title.ToLower().Contains(titleLower))
                continue;

            if (author != null && author.Trim() != "")
            {
                if (!book.Author.ToLower().Contains(author.ToLower()))
                    continue;
            }

            result.Add(book);
        }

        return Ok(result);
    }

    [HttpPost]
    public IActionResult CreateBook(Book book)
    {
        var role = Request.Headers["x-user-role"].ToString();

        if (role != "admin")
            return StatusCode(403, new { error = "Access denied" });

        if (string.IsNullOrWhiteSpace(book.Title))
          return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(book.Author))
            return BadRequest("Author is required");

        if (book.Year < 1800)
            return BadRequest("Year must be 1800 or later");

        book.Id = Books.Any() ? Books.Max(b => b.Id) + 1 : 1;
        Books.Add(book);

        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateBook(int id, Book updatedBook)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound("Book not found");

        if (string.IsNullOrWhiteSpace(updatedBook.Title))
            return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(updatedBook.Author))
            return BadRequest("Author is required");

        if (updatedBook.Year < 1800)
            return BadRequest("Year must be 1800 or later");

        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.Year = updatedBook.Year;

        return Ok(book);
    }


    [HttpDelete("{id:int}")]
    public IActionResult DeleteBook(int id)
    {
        var role = Request.Headers["x-user-role"].ToString();

        if (role != "admin")
            return StatusCode(403, new { error = "Access denied" });

        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound("Book not found");

        Books.Remove(book);
        return NoContent();
    }

}