namespace FirstBlazorApp.Services.IService
{
    public interface IRolePermissionChecker
    {
        Task<bool> HasPermissionAsync(string actionMethodName);
        void ClearRoleCache(int roleId);


    }
}
