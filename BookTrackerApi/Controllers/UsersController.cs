using BookTracker;
using BookTracker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserBusiness _userBusiness;

    public UsersController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        return Ok(_userBusiness.GetAllUsers());
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = _userBusiness.GetUserById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create([FromBody] User user)
    {
        _userBusiness.AddUser(user);
        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        var existing = _userBusiness.GetUserById(id);
        if (existing is null)
        {
            return NotFound();
        }

        user.UserId = id;
        _userBusiness.UpdateUser(user);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _userBusiness.GetUserById(id);
        if (existing is null)
        {
            return NotFound();
        }

        _userBusiness.RemoveUser(id);
        return NoContent();
    }
}
