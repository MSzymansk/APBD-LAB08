using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface IClientService
{
    
    Task<IActionResult> AddClient(CreateClientDTO client);
    
    
}