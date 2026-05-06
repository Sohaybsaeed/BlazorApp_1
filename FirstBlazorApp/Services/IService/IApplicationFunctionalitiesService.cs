using FirstBlazorApp.Models.Custom_Models;
namespace FirstBlazorApp.Services.IService
{
    public interface IApplicationFunctionalitiesService : IDisposable
    {
        Task<List<PermissionResponse>> GetFunctionalitiesByFormIdAsync(int FormId, string UserId);
    }
}