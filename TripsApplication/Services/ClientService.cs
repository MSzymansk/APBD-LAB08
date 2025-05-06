using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public class ClientService : IClientService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Encrypt=False;";

    public async Task<bool> ClientExist(int idClient)
    {
        // Check if client with given ID exists
        string command = @"SELECT COUNT(*) FROM Client WHERE IdClient = @IdClient";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);

            await conn.OpenAsync();

            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }

    public async Task<List<TripWithRegistrationDTO>> GetTrips(int idClient)
    {
        var trips = new List<TripWithRegistrationDTO>();

        // Get trips for the given client, including registration and payment dates
        string command = @"
        SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate
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
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripWithRegistrationDTO()
                    {
                        IdTrip = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        Registration = new RegistrationDto()
                        {
                            RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                            PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                                ? null
                                : reader.GetInt32(reader.GetOrdinal("PaymentDate"))
                        }
                    });
                }
            }
        }

        return trips;
    }

    public async Task<bool> EmailExists(string email)
    {
        // Check if email already exists in Client table
        string command = "SELECT COUNT(*) FROM Client WHERE Email = @Email";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Email", email);
            await conn.OpenAsync();

            int result = (int)await cmd.ExecuteScalarAsync();
            return result == 0;
        }
    }

    public async Task<bool> PeselExists(string pesel)
    {
        // Check if PESEL already exists in Client table
        string command = "SELECT COUNT(*) FROM Client WHERE Pesel = @Pesel";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Pesel", pesel);
            await conn.OpenAsync();

            int result = (int)await cmd.ExecuteScalarAsync();
            return result == 0;
        }
    }

    public async Task<bool> AddClient(CreateClientDTO client)
    {   
        // Insert a new client and return new client ID
        string command = @"
            INSERT  INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            VALUES   (@FirstName, @LastName, @Email, @Telephone, @Pesel);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
    }
}