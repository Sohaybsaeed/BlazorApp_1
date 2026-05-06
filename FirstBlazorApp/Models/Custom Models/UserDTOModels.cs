using System.ComponentModel.DataAnnotations;

namespace FirstBlazorApp.Models.Custom_Models
{
    public class UserPermissionDTO
    {
        public int FormId { get; set; }
        public string UserId { get; set; }
        public List<int> FunctionId { get; set; }
        public bool FullAccess { get; set; }
    }
    public class GetUserFunctionalitiesDTO
    {
        public string UserId { get; set; }
        public int FormId { get; set; }
    }
    public class Permission
    {
        public string FunctionalityName { get; set; }
        public int FunctionalityId { get; set; }
        public bool? IsSelected { get; set; }
    }
    public class AppFunctionality
    {
        public int Id { get; set; }
        public string FunctionalityName { get; set; }
        public bool IsAllow { get; set; }
    }
    public class CustomUserAccess
    {
        public int? FunctionalityId { get; set; }
        public string FunctionalityName { get; set; }
        public bool? AllowAccess { get; set; }
        public bool? IsFullAccess { get; set; }
    }
    public class PermissionResponse
    {
        public List<Permission> list { get; set; }
        public bool? IsFullAccess { get; set; }
    }

    public class UserPermissionsModel
    {
        public int formId { get; set; }
        public string FunctionalityName { get; set; }
        public bool? FullAccess { get; set; }
        public bool? AllowAccess { get; set; }
        public string FormName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string ActionMethodName { get; set; }
    }
    public class CreatePassword
    {
        public string UserId { get; set; }
    }
    public class CreatePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password does not match")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }

    }


    public class CreateNewPasswordModel
    {

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password does not match")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }



    }



    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    //public class UserLoginResponse
    //{
    //    public bool Succeeded { get; set; }
    //    public bool IsUserExists { get; set; }
    //    public string Name { get; set; }
    //    public int UserId { get; set; }
    //    public string UserRole { get; set; }
    //    public string Error { get; set; }
    //    public int RoleTemplateId { get; set; }
    //    public string BranchCode { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }

    //}
}
