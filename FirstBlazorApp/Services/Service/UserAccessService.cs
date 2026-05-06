using FirstBlazorApp.Models.Custom_Models;
using FirstBlazorApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetailChannel.Models.CustomModel;
using RetailChannel.Models.Model;

namespace FirstBlazorApp.Services.Service
{
    public class UserAccessService : IUserAccessService
    {
        private bool alreadyDisposed = false;
        public IBRetailDbContext _ctx;
        private readonly UserManager<ApplicationUser> _um;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool isDisposing)
        {
            if (alreadyDisposed)
                return;
            alreadyDisposed = true;
        }
        
        public UserAccessService(UserManager<ApplicationUser> um, IBRetailDbContext ctx)
        {
            _ctx = ctx;
            _um = um;
        }

        public List<ApplicationUser> GetAllUsersAsync()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            users = _um.Users.ToList();
            return users;
        }

        public async Task<bool> ChangePermissionAsync(UserPermissionDTO model)
        {
            string FormName;
            List<UserAccess> accesslist = new List<UserAccess>();

            FormName = await _ctx.FormDetail.Where(x => x.Id == model.FormId).Select(x => x.FormName).SingleOrDefaultAsync();
            _ctx.UserAccess.RemoveRange(_ctx.UserAccess.Where(x => x.UserId == model.UserId && x.FormName == FormName));
            await _ctx.SaveChangesAsync();
            if (model.FunctionId == null && model.FullAccess == false)
            {
                return true;
            }
            else
            {
                if (model.FunctionId == null && model.FullAccess)
                {
                    accesslist.Add(new UserAccess { AllowAccess = true, ApplicationFunctionalityId = 0, FormName = FormName, FullAccess = model.FullAccess, UserId = model.UserId });
                    _ctx.UserAccess.AddRange(accesslist);
                    await _ctx.SaveChangesAsync();
                }
                else
                {
                    if (model.FunctionId.Count > 0 || model.FullAccess)
                    {
                        if (model.FunctionId.Count == 0 && model.FullAccess)
                        {
                            accesslist.Add(new UserAccess { AllowAccess = false, ApplicationFunctionalityId = 0, FormName = FormName, FullAccess = model.FullAccess, UserId = model.UserId });
                        }
                        else
                        {
                            foreach (int id in model.FunctionId)
                            {
                                accesslist.Add(new UserAccess { AllowAccess = true, ApplicationFunctionalityId = id, FormName = FormName, FullAccess = model.FullAccess, UserId = model.UserId });
                            }
                        }
                    }
                    _ctx.UserAccess.AddRange(accesslist);
                    await _ctx.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<List<UserPermissionsModel>> GetUserPermissionsAsync(string userid)
        {
            List<UserPermissionsModel> permissions = new List<UserPermissionsModel>();
            permissions = await (from ua in _ctx.UserAccess
                                 join af in _ctx.ApplicationFunctionalities on ua.ApplicationFunctionalityId equals af.Id into ps
                                 from af in ps.DefaultIfEmpty()
                                 where ua.UserId == userid
                                 select new UserPermissionsModel
                                 {
                                     FunctionalityName = af.FunctionalityName,
                                     FullAccess = ua.FullAccess,
                                     AllowAccess = ua.AllowAccess,
                                     FormName = ua.FormName,
                                     ActionMethodName = af.ActionMethodName
                                 }
                           ).ToListAsync();
            return permissions;
        }
        public async Task<List<UserPermissionsModel>> GetUserPermissionsByRoleIdAsync(int RoleId)
        {
            List<UserPermissionsModel> permissions = new List<UserPermissionsModel>();
            permissions = await (from ua in _ctx.PermissionTemplateDetail
                                 join af in _ctx.ApplicationFunctionalities on ua.FunctionalityId equals af.Id into ps
                                 from af in ps.DefaultIfEmpty()
                                 where ua.TemplateId == RoleId
                                 select new UserPermissionsModel
                                 {
                                     formId = (int)af.FormId,
                                     FunctionalityName = af.FunctionalityName,
                                     FullAccess = false,
                                     AllowAccess = ua.IsAllow,
                                     FormName = ua.FormName,
                                     ActionMethodName = af.ActionMethodName
                                 }).ToListAsync();
            return permissions;
        }
        public async Task<List<Items>> GetMenu(int RoleId)
        {
            List<Items> items = new List<Items>() { };
            var FormDetails = await _ctx.FormDetail.ToListAsync();

            items = await (from f1 in _ctx.FormDetail
                           join Appfunc in _ctx.ApplicationFunctionalities on f1.Id equals Appfunc.FormId
                           join PTD in _ctx.PermissionTemplateDetail on Appfunc.Id equals PTD.FunctionalityId
                           where Appfunc.IsMenuItem == true
                           group f1 by f1.DisplayName into g
                           orderby g.FirstOrDefault().DisplayOrder ascending
                           select new Items
                           {
                               ItemName = g.FirstOrDefault().DisplayName,
                               Icon = g.FirstOrDefault().IconCode,
                               SubMenuItems = (
                               from Af in _ctx.ApplicationFunctionalities
                               join f2 in _ctx.FormDetail on Af.FormId equals f2.Id
                               join f5 in _ctx.PermissionTemplateDetail on Af.Id equals f5.FunctionalityId
                               where Af.IsMenuItem == true && Af.MenuReferenceName == g.FirstOrDefault().DisplayName && f5.TemplateId == RoleId && f2.IsActive == true
                               select new SubMenuItems
                               {
                                   submenuItem = Af.FunctionalityName,
                                   NavigationLink = "/" + f2.ControllerName + "/" + Af.ActionMethodName
                               }
                               ).ToList()
                           }
                           ).ToListAsync();

            return items;
        }

        // ✅ NEW METHOD: Fast permission check (Role -> PermissionTemplateDetail -> ApplicationFunctionalities)
        public async Task<bool> RoleHasActionAsync(int roleId, string actionMethodName)
        {
            if (string.IsNullOrWhiteSpace(actionMethodName))
                return false;

            // If IsAllow is int (0/1) in DB, replace ua.IsAllow == true with ua.IsAllow == 1
            return await (from ptd in _ctx.PermissionTemplateDetail
                          join af in _ctx.ApplicationFunctionalities
                            on ptd.FunctionalityId equals af.Id
                          where ptd.TemplateId == roleId
                                && ptd.IsAllow
                                && af.ActionMethodName == actionMethodName
                          select ptd.Id).AnyAsync();
        }

    }
}