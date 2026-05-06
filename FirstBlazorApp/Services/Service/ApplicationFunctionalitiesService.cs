using FirstBlazorApp.Services.IService;
using Microsoft.EntityFrameworkCore;
using FirstBlazorApp.Models.Custom_Models;
namespace FirstBlazorApp.Services.Service
{
    public class ApplicationFunctionalitiesService : IApplicationFunctionalitiesService
    {
        private bool alreadyDisposed = false;
        public IBRetailDbContext _ctx;
        public ApplicationFunctionalitiesService(IBRetailDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<PermissionResponse>> GetFunctionalitiesByFormIdAsync(int FormId, string UserId)
        {
            List<PermissionResponse> response = new List<PermissionResponse>();
            List<AppFunctionality> function = new List<AppFunctionality>();
            List<CustomUserAccess> access = new List<CustomUserAccess>();
            string FormName = "";

            function = await (from fun in _ctx.ApplicationFunctionalities
                              join form in _ctx.FormDetail on fun.FormId equals form.Id
                              where form.Id == FormId
                              select new AppFunctionality
                              {
                                  Id = fun.Id,
                                  FunctionalityName = fun.FunctionalityName,
                              }).ToListAsync();

            FormName = await _ctx.FormDetail.Where(x => x.Id == FormId).Select(x => x.FormName).SingleOrDefaultAsync();


            access = await (from acc in _ctx.UserAccess
                            join func in _ctx.ApplicationFunctionalities on acc.ApplicationFunctionalityId equals func.Id
                            where acc.UserId == UserId && acc.FormName == FormName
                            select new CustomUserAccess
                            {
                                FunctionalityId = acc.ApplicationFunctionalityId,
                                FunctionalityName = func.FunctionalityName,
                                AllowAccess = acc.AllowAccess,
                                IsFullAccess = acc.FullAccess
                            }).ToListAsync();

            if (access.Count == 0)
            {
                List<Permission> per = new List<Permission>();
                foreach (var item in function)
                {
                    per.Add(new Permission { FunctionalityId = item.Id, FunctionalityName = item.FunctionalityName, IsSelected = false });
                }
                bool? IsFullAccess = false;

                IsFullAccess = await _ctx.UserAccess.Where(x => x.UserId == UserId && x.FormName == FormName).Select(x => x.FullAccess).SingleOrDefaultAsync();
                response.Add(new PermissionResponse { IsFullAccess = IsFullAccess, list = per });
            }
            else
            {
                List<Permission> per = new List<Permission>();

                List<int?> ActiveIds = access.Select(x => x.FunctionalityId).ToList();
                foreach (var item in function.Where(t => ActiveIds.Contains(t.Id)))
                {
                    item.IsAllow = true;
                }
                foreach (var item in function)
                {
                    per.Add(new Permission { FunctionalityId = item.Id, FunctionalityName = item.FunctionalityName, IsSelected = item.IsAllow });
                }
                //foreach (var item in access)
                //{

                //    per.Add(new Permission { FunctionalityId = item.FunctionalityId, FunctionalityName = item.FunctionalityName, IsSelected = item.AllowAccess });
                //}
                response.Add(new PermissionResponse { IsFullAccess = access.Where(x => x.IsFullAccess == true).Count() > 0, list = per });
            }
            return response;
        }

        protected virtual void Dispose(bool isDisposing)
        {

            if (alreadyDisposed)
                return;
            alreadyDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
