using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IClientService _clientService, ITripService _tripService) : ControllerBase
    {
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            if (!await _clientService.ClientExist(id))
            {
                return NotFound($"Client id: {id} not found");
            }

            var trips = await _clientService.GetTrips(id);

            if (trips.Count == 0)
            {
                return NotFound($"Client id: {id} has no trips");
            }

            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] CreateClientDTO client)
        {
            if (client == null)
            {
                return BadRequest("Client is null");
            }

            return await _clientService.AddClient(client);
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> registerClient(int id, int tripId)
        {
            if (!await _clientService.ClientExist(id))
            {
                return NotFound($"Client id: {id} not found");
            }

            if (!await _tripService.TripExist(tripId))
            {
                return NotFound($"Trip id: {tripId} not found");
            }

            if (!await _tripService.SpotsExist(tripId))
            {
                return Conflict($"Spot id: {tripId} is full");
            }

            if (await _tripService.RegisterExists(id, tripId))
            {
                return Conflict($"Client id: {id} is already registered");
            }

            return await _tripService.RegisterClient(id, tripId);
        }
        
        
    }
}