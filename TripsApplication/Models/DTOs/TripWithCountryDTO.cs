namespace WebApplication1.Models.DTOs;

public class TripWithCountryDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO> Countries { get; set; }
}

public class CountryDTO
{
    public int IdCountry { get; set; }
    public string Name { get; set; }
}