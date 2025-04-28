using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public class TripService : ITripService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Encrypt=False;";

    
    public async Task<List<TripDTO>> GetAllTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name FROM Trip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                    });
                }
            }
        }
        

        return trips;
    }
}