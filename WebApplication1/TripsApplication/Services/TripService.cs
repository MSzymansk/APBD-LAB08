using Microsoft.AspNetCore.Mvc;
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

    public async Task<bool> ClientExist(int idClient)
    {
        int count = 0;

        string command = @"
        SELECT COUNT(*) AS Count
        FROM Trip t
        INNER JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
        WHERE ct.IdClient = @IdClient";


        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);

            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    count = reader.GetInt32(reader.GetOrdinal("Count"));
                }
            }

            return count > 0;
        }
    }

    public async Task<bool> TripExist(int idTrip)
    {
        string command = @"SELECT COUNT(*) FROM Trip WHERE IdTrip = @IdTrip";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);

            await conn.OpenAsync();

            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }

    public async Task<bool> SpotsExist(int idTrip)
    {
        string command = @"
        SELECT 
            (t.MaxPeople - COUNT(ct.IdClient)) AS FreeSpots
        FROM Trip t
        LEFT JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
        WHERE t.IdTrip = @IdTrip
        GROUP BY t.MaxPeople
    ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            await conn.OpenAsync();

            object result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return false;
            }

            int freeSpots = Convert.ToInt32(result);
            return freeSpots > 0;
        }
    }

    public async Task<bool> RegisterExists(int idClient, int idTrip)
    {
        string query = "SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";

        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }

    public async Task<bool> RegisterClient(int idClient, int idTrip)
    {
        String command = @"
        INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt)
        VALUES(@IdClient, @IdTrip, @RegisteredAt);
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            cmd.Parameters.AddWithValue("@RegisteredAt", DateTime.Now.ToString("yyyyMMdd"));

            await conn.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            
            return rowsAffected > 0;
        }
    }

    public async Task<bool> RemoveClientFromTrip(int clientId, int tripId)
    {
        string query = "DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", clientId);
            cmd.Parameters.AddWithValue("@IdTrip", tripId);
            await conn.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}