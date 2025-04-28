using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface ITripService
{
    Task<List<TripDTO>>  GetAllTrips();
}