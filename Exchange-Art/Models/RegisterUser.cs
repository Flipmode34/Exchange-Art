using System.ComponentModel.DataAnnotations;

namespace Exchange_Art.Models
{
    /*
     * Class that sits on top of the ApplicationUser class,
     * to be able to get a login form that just takes
     * a name, emailadres and password.
     */
    public class RegisterUser
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

    }
}
