using System.ComponentModel.DataAnnotations;

namespace Exchange_Art.Models
{
    /*
     * Class that sits on top of the ApplicationUser class,
     * to be able to get a login form that just takes
     * a name, emailadres and password.
     */
    public class User
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
