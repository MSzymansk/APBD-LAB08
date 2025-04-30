using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IClientService _clientService) : ControllerBase
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
        public async Task<IActionResult> AddClient([FromBody]CreateClientDTO client)
        {
            if (client == null)
            {
                return BadRequest("Client is null");
            }

            var result = await _clientService.AddClient(client);
            return result;
        }
    }
}