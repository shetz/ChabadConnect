using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        public RegisterDto(string username, string knownAs, string gender, DateTime dateOfBirth, string city, string country, string password)
        {
            this.Username = username;
            this.KnownAs = knownAs;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.City = city;
            this.Country = country;
            this.Password = password;

        }
        
        [Required] public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}