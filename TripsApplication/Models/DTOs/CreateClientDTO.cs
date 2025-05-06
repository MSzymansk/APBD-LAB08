using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs;

public class CreateClientDTO
{
    [Required(ErrorMessage = "First name is required.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telephone number is required.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Telephone number must contain only digits.")]
    public string Telephone { get; set; }

    [Required(ErrorMessage = "PESEL is required.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must contain only 11 digits.")]
    public string Pesel { get; set; }
}