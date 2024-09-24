using System.ComponentModel.DataAnnotations;

namespace Talabat.API.Dots
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]

        public string FirstName { get; set; }

        [Required]


        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string role { get; set; }


        [Required]
        //[RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        //    ErrorMessage = "Password must have 1 uppercase, 1 lowercase , 1 number , 1 non alphanumeric and at least 6 characters")]
        public string Password { get; set; }


        public string Country { get; set; }


        public string City { get; set; }


        public string Street { get; set; }
    }
}
