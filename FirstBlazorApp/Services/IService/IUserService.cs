using FirstBlazorApp.Models.Custom_Models;

public interface IUserService
{
    Task<UserLoginResponse> LoginAsync(string Email, string Password);
    Task<List<ApplicationUser>> GetUsersAsync();
    Task<List<RoleTemplateDD>> GetRoleTemplates();
    Task<ActionResponse> CreateUserAsync(ApplicationUser request);
    Task<ApplicationUser?> FindUserByUserIdAsync(string userId);
    Task<ActionResponse> UpdateUserAsync(ApplicationUser request);

}
