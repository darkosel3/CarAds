using System.ComponentModel.DataAnnotations;

namespace CarAds.Models
{
    public class User
    {
        [Required(ErrorMessage = "Username is required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
