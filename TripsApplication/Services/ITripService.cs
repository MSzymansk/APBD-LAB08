using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface ITripService
{
    Task<List<TripWithCountryDTO>> GetAllTrips();

    Task<bool> TripExist(int idTrip);

    Task<bool> SpotsExist(int idTrip);

    Task<bool> RegisterExists(int idClient, int idTrip);

    Task<bool> RegisterClient(int idClient, int idTrip);

    Task<bool> RemoveClientFromTrip(int clientId, int tripId);
}