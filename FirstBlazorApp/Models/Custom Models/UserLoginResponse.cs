using System.Security.Claims;

namespace FirstBlazorApp.Models.Custom_Models
{
    public class UserLoginResponse
    {
        public bool Succeeded { get; set; }
        public bool IsUserExists { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; }
        public string Error { get; set; }
        public int RoleTemplateId { get; set; }
        public string BranchCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Claim> Claims { get; set; } = default!;
        public List<KeyValuePair<string, string>> SessionData { get; set; } = new();
    }
}
