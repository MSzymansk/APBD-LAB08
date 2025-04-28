using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface ITripService
{
    Task<List<TripWithCountryDTO>> GetAllTrips();
    Task<bool> ClientExist(int idClient);
    Task<List<TripWithRegistrationDTO>> GetTrips(int idClient);
}