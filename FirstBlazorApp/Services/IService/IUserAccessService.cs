using FirstBlazorApp.Models;
using FirstBlazorApp.Models.Custom_Models;
using RetailChannel.Models.CustomModel;

namespace FirstBlazorApp.Services.IService
{
    public interface IUserAccessService : IDisposable
    {
        List<ApplicationUser> GetAllUsersAsync();
        Task<bool> ChangePermissionAsync(UserPermissionDTO dto);
        Task<List<UserPermissionsModel>> GetUserPermissionsAsync(string userid);
        Task<List<UserPermissionsModel>> GetUserPermissionsByRoleIdAsync(int RoleId);
        Task<List<Items>> GetMenu(int RoleId);
        Task<bool> RoleHasActionAsync(int roleId, string actionMethodName);

    }
}