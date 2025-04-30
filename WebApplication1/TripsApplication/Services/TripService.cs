using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public class TripService : ITripService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Encrypt=False;";


    public async Task<List<TripWithCountryDTO>> GetAllTrips()
    {
        var trips = new List<TripWithCountryDTO>();
        var tripCountries = new Dictionary<int, List<CountryDTO>>();

        string command = @"
        SELECT t.IdTrip, c.IdCountry as CountryId, c.Name AS CountryName
        FROM Trip t
        INNER JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
        INNER JOIN Country c ON ct.IdCountry = c.IdCountry";


        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var idTrip = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    var idCountry = reader.GetInt32(reader.GetOrdinal("CountryId"));
                    var countryName = reader.GetString(reader.GetOrdinal("CountryName"));

                    if (!tripCountries.ContainsKey(idTrip))
                    {
                        tripCountries[idTrip] = new List<CountryDTO>();
                    }

                    tripCountries[idTrip].Add(new CountryDTO() { IdCountry = idCountry, Name = countryName });
                }
            }
        }

        foreach (var trip in tripCountries)
        {
            string tripCommand = @"
                    SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople
                    FROM Trip t
                    WHERE t.IdTrip = @TripKey";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(tripCommand, conn))
            {
                cmd.Parameters.AddWithValue("@TripKey", trip.Key);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        trips.Add(new TripWithCountryDTO()
                        {
                            IdTrip = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = trip.Value
                        });
                    }
                }
            }
        }

        return trips;
    }
}