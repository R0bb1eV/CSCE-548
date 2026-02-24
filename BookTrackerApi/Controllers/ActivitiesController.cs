using BookTracker;
using BookTracker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityBusiness _activityBusiness;

    public ActivitiesController(IActivityBusiness activityBusiness)
    {
        _activityBusiness = activityBusiness;
    }

    [HttpGet]
    public ActionResult<List<Activity>> GetAll()
    {
        return Ok(_activityBusiness.GetAllActivities());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Activity> GetById(int id)
    {
        var activity = _activityBusiness.GetActivityById(id);
        return activity is null ? NotFound() : Ok(activity);
    }

    [HttpPost]
    public ActionResult<Activity> Create([FromBody] Activity activity)
    {
        _activityBusiness.AddActivity(activity);
        return CreatedAtAction(nameof(GetById), new { id = activity.ActivityId }, activity);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Activity activity)
    {
        var existing = _activityBusiness.GetActivityById(id);
        if (existing is null)
        {
            return NotFound();
        }

        activity.ActivityId = id;
        _activityBusiness.UpdateActivity(activity);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _activityBusiness.GetActivityById(id);
        if (existing is null)
        {
            return NotFound();
        }

        _activityBusiness.RemoveActivity(id);
        return NoContent();
    }
}
