using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController(ITripService tripsService) : ControllerBase
    {   
        /* GET /api/trips - Returns list of all trips. */
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await tripsService.GetAllTrips();
            return Ok(trips);
        }
    }
}