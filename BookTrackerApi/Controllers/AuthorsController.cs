using BookTracker;
using BookTracker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorBusiness _authorBusiness;

    public AuthorsController(IAuthorBusiness authorBusiness)
    {
        _authorBusiness = authorBusiness;
    }

    [HttpGet]
    public ActionResult<List<Author>> GetAll()
    {
        return Ok(_authorBusiness.GetAllAuthors());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Author> GetById(int id)
    {
        var author = _authorBusiness.GetAuthorById(id);
        return author is null ? NotFound() : Ok(author);
    }

    [HttpPost]
    public ActionResult<Author> Create([FromBody] Author author)
    {
        _authorBusiness.AddAuthor(author);
        return CreatedAtAction(nameof(GetById), new { id = author.AuthorId }, author);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Author author)
    {
        var existing = _authorBusiness.GetAuthorById(id);
        if (existing is null)
        {
            return NotFound();
        }

        author.AuthorId = id;
        _authorBusiness.UpdateAuthor(author);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _authorBusiness.GetAuthorById(id);
        if (existing is null)
        {
            return NotFound();
        }

        _authorBusiness.RemoveAuthor(id);
        return NoContent();
    }
}
