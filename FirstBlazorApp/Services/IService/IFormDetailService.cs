using FirstBlazorApp.Models;

namespace FirstBlazorApp.Service.IServices
{
    public interface IFormDetailService : IDisposable
    {
        // Task<List<FormDetail>> GetAllFormsAsync();
        Task<List<FormDetail>> GetAllFormsAsync();
    }
}