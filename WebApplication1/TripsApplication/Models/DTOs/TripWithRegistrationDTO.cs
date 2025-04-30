namespace WebApplication1.Models.DTOs;

public class TripWithRegistrationDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int MaxPeople { get; set; }

    public RegistrationDto Registration { get; set; }

}

public class RegistrationDto
{
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}