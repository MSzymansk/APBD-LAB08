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

    public async Task<List<TripWithRegistrationDTO>> GetTrips(int idClient)
    {
        var trips = new List<TripWithRegistrationDTO>();

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

    private async Task<IActionResult> ValidateClientData(CreateClientDTO client)
    {
        if (string.IsNullOrWhiteSpace(client.FirstName))
        {
            return new BadRequestObjectResult("First name is required.");
        }

        if (!client.FirstName.All(char.IsLetter))
        {
            return new BadRequestObjectResult("First name must contain only letters.");
        }

        if (string.IsNullOrWhiteSpace(client.LastName))
        {
            return new BadRequestObjectResult("Last name is required.");
        }

        if (!client.LastName.All(char.IsLetter))
        {
            return new BadRequestObjectResult("Last name must contain only letters.");
        }

        if (string.IsNullOrWhiteSpace(client.Email) || !client.Email.Contains('@'))
        {
            return new BadRequestObjectResult("Invalid email format.");
        }

        if (string.IsNullOrWhiteSpace(client.Telephone))
        {
            return new BadRequestObjectResult("Telephone number is required.");
        }

        if (!client.Telephone.All(char.IsDigit))
        {
            return new BadRequestObjectResult("Telephone number must contain only digits.");
        }

        if (string.IsNullOrWhiteSpace(client.Pesel) || client.Pesel.Length != 11 || !client.Pesel.All(char.IsDigit))
        {
            return new BadRequestObjectResult("Invalid PESEL number. It must be exactly 11 digits long.");
        }

        return new OkResult();
    }

    private async Task<IActionResult> ValidateEmail(string email)
    {
        string command = "SELECT COUNT(*) FROM Client WHERE Email = @Email";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Email", email);
            await conn.OpenAsync();

            var result = (int)await cmd.ExecuteScalarAsync();
            if (result > 0)
            {
                return new ConflictObjectResult("Email is already in use.");
            }
        }

        return new OkResult();
    }

    private async Task<IActionResult> ValidatePesel(string pesel)
    {
        string command = "SELECT COUNT(*) FROM Client WHERE Pesel = @Pesel";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@Pesel", pesel);
            await conn.OpenAsync();

            var result = (int)await cmd.ExecuteScalarAsync();
            if (result > 0)
            {
                return new ConflictObjectResult("Pesel is already in use.");
            }
        }

        return new OkResult();
    }
    
    public async Task<IActionResult> AddClient(CreateClientDTO client)
    {
        var validationResult = await ValidateClientData(client);
        if (validationResult is BadRequestObjectResult)
        {
            return validationResult;
        }

        var emailValidationResult = await ValidateEmail(client.Email);
        if (emailValidationResult is ConflictObjectResult)
        {
            return emailValidationResult;
        }

        var peselValidationResult = await ValidatePesel(client.Pesel);
        if (peselValidationResult is ConflictObjectResult)
        {
            return peselValidationResult;
        }


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
            
            var newClientId = (int)await cmd.ExecuteScalarAsync();
            return new CreatedResult($"/Client/{newClientId}", newClientId);
        }
    }
}