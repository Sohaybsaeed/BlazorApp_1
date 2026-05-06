using System.ComponentModel.DataAnnotations;

namespace FirstBlazorApp.Models.Custom_Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }


    
}