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
            
            if (!await _clientService.EmailExists(client.Email))
            {
                return Conflict(new { message = "Email is already in use." });
            }

            if (!await _clientService.PeselExists(client.Pesel))
            {
                return Conflict(new { message = "Pesel is already in use." });
            }
            

            bool success = await _clientService.AddClient(client);

            return success
                ? Created("", new { message = "Client created successfully." })
                : StatusCode(500, "An error occurred while adding the client.");
        }

        [HttpPut("{idClient}/trips/{idTrip}")]
        public async Task<IActionResult> registerClient(int idClient, int idTrip)
        {
            if (!await _clientService.ClientExist(idClient))
            {
                return NotFound($"Client id: {idClient} not found");
            }

            if (!await _tripService.TripExist(idTrip))
            {
                return NotFound($"Trip id: {idTrip} not found");
            }

            if (!await _tripService.SpotsExist(idTrip))
            {
                return Conflict($"Trip id: {idTrip} is full");
            }

            if (await _tripService.RegisterExists(idClient, idTrip))
            {
                return Conflict($"Client id: {idClient} is already registered");
            }

            bool success = await _tripService.RegisterClient(idClient, idTrip);

            return success
                ? new CreatedResult($"/api/trips/{idTrip}/clients/{idClient}",
                    $"Client id: {idClient} registered to the trip id: {idTrip}")
                : StatusCode(500, "Failed to register client to the trip");
        }

        [HttpDelete("{idClient}/trips/{idTrip}")]
        public async Task<IActionResult> deleteClient(int idClient, int idTrip)
        {
            if (!await _clientService.ClientExist(idClient))
            {
                return NotFound($"Client id: {idClient} not found");
            }

            if (!await _tripService.TripExist(idTrip))
            {
                return NotFound($"Trip id: {idTrip} not found");
            }

            if (!await _tripService.RegisterExists(idClient, idTrip))
            {
                return NotFound($"Client id: {idClient} is not registered to the trip id: {idTrip}");
            }

            bool success = await _tripService.RemoveClientFromTrip(idClient, idTrip);

            return success
                ? Ok($"Client id: {idClient} unregistered from trip id: {idTrip}")
                : StatusCode(500, "An error occurred while unregistering client from trip");
        }
    }
}