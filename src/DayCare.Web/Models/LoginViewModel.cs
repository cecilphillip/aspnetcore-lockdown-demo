namespace DayCare.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Login Type")]
        public LoginType LoginType { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

    }

    public enum LoginType
    {
        Staff,
        Guardian
    }

}
