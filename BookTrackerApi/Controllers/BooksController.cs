using BookTracker;
using BookTracker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookBusiness _bookBusiness;

    public BooksController(IBookBusiness bookBusiness)
    {
        _bookBusiness = bookBusiness;
    }

    [HttpGet]
    public ActionResult<List<Book>> GetAll()
    {
        return Ok(_bookBusiness.GetAllBooks());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = _bookBusiness.GetBookById(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public ActionResult<Book> Create([FromBody] Book book)
    {
        _bookBusiness.AddBook(book);
        return CreatedAtAction(nameof(GetById), new { id = book.ID }, book);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Book book)
    {
        var existing = _bookBusiness.GetBookById(id);
        if (existing is null)
        {
            return NotFound();
        }

        book.ID = id;
        _bookBusiness.UpdateBook(book);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _bookBusiness.GetBookById(id);
        if (existing is null)
        {
            return NotFound();
        }

        _bookBusiness.RemoveBook(id);
        return NoContent();
    }
}
