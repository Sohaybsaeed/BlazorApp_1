using FirstBlazorApp.Models;
using Microsoft.EntityFrameworkCore;
using FirstBlazorApp.Service.IServices;
namespace RetailChannel.Service.Services
{
    public class FormDetailService : IFormDetailService
    {
        private bool alreadyDisposed = false;
        private readonly IBRetailDbContext _ctx;
        public FormDetailService(IBRetailDbContext dbcontext)
        {
            _ctx = dbcontext;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<List<FormDetail>> GetAllFormsAsync()
        {
            List<FormDetail> forms = new List<FormDetail>();
            forms = await _ctx.FormDetail.ToListAsync();
            return forms;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (alreadyDisposed)
                return;
            alreadyDisposed = true;
        }
    }
}