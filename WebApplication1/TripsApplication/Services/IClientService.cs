using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface IClientService
{
    Task<bool> AddClient(CreateClientDTO client);
    Task<bool> ClientExist(int idClient);
    Task<bool> EmailExists(string email);
    Task<bool> PeselExists(string pesel);
    Task<List<TripWithRegistrationDTO>> GetTrips(int idClient);
}