using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripsService;
        private readonly IClientService _clientService;

        public TripsController(ITripService tripsService, IClientService clientService)
        {
            _tripsService = tripsService;
            _clientService = clientService;
        }
        

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetAllTrips();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrips(int id)
        {
            if (!await _tripsService.ClientExist(id))
            {
                return NotFound($"Client id: {id} not found");
            }

            var trips = await _tripsService.GetTrips(id);

            if (trips.Count == 0)
            {
                return NotFound($"Client id: {id} has no trips");
            }
            
            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient(CreateClientDTO client)
        {

            if (client == null)
            {
                return BadRequest("Client is null");
            }

            var result =  await _clientService.AddClient(client);
            return result;
        }
        
        
    }
}