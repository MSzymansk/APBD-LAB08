using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripsService;

        public TripsController(ITripService tripsService)
        {
            _tripsService = tripsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetAllTrips();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            // if( await DoesTripExist(id)){
            //  return NotFound();
            // }
            // var trip = ... GetTrip(id);
            return Ok();
        }
    }
}
