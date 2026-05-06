using FirstBlazorApp.Models;
using FirstBlazorApp.Models.Custom_Models;
using FirstBlazorApp.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace FirstBlazorApp.Services.Service
{
    public class PermissionTemplateService : IPermissionTemplateService
    {
        private bool alreadyDisposed = false;
        private readonly IBRetailDbContext ctx;


        public PermissionTemplateService(IBRetailDbContext dbcontext)
        {
            ctx = dbcontext;

        }
        public async Task<CustomResponse<List<PermissionTemplate>>> GetAllPermissionTemplate()
        {
            var result = await ctx.PermissionTemplate.Select(x => new PermissionTemplate { Id = x.Id, TemplateName = x.TemplateName }).ToListAsync();
            return new CustomResponse<List<PermissionTemplate>>() { IsSuccess = true, Message = "Get All Templates", Data = result };
        }
        public async Task<CustomResponse<List<PermissionTemplate>>> GetAllActivePermissionTemplate()
        {
            var result = await ctx.PermissionTemplate.Where(x => x.IsActive == true).Select(x => new PermissionTemplate { Id = x.Id, TemplateName = x.TemplateName }).ToListAsync();
            return new CustomResponse<List<PermissionTemplate>>() { IsSuccess = true, Message = "Get All Templates", Data = result };
        }
        public List<PermissionTemplateDetails> GetAllFunctionalitiesAsync()
        {
            List<PermissionTemplateDetails> response = new List<PermissionTemplateDetails>();

            response = (from fun in ctx.ApplicationFunctionalities
                        join form in ctx.FormDetail on fun.FormId equals form.Id
                        where form.IsActive == true && fun.IsActive == true
                        select new PermissionTemplateDetails
                        {
                            FormId = form.Id,
                            FormName = form.FormName,
                            FormDisplayName = form.DisplayName,
                            FunctionalityId = fun.Id,
                            FunctionalityName = fun.FunctionalityName,
                            IsAllow = false
                        }).ToList();

            return response;
        }
        public async Task<CustomResponse<PermissionTemplateViewModel>> GetPermissionTemplateById(int TemplateId)
        {
            PermissionTemplateViewModel viewModel = new PermissionTemplateViewModel();
            try
            {

                var templateDetail = await ctx.PermissionTemplate.Where(x => x.Id == TemplateId).FirstOrDefaultAsync();
                if (templateDetail != null)
                {
                    var activePermissions = await ctx.PermissionTemplateDetail.Where(x => x.TemplateId == TemplateId).Select(z => z.FunctionalityId).ToListAsync();
                    var response = (from fun in ctx.ApplicationFunctionalities
                                    join form in ctx.FormDetail on fun.FormId equals form.Id
                                    where form.IsActive == true && fun.IsActive == true
                                    select new PermissionTemplateDetails
                                    {
                                        FormId = form.Id,
                                        FormName = form.FormName,
                                        FormDisplayName = form.DisplayName,
                                        FunctionalityId = fun.Id,
                                        FunctionalityName = fun.FunctionalityName,
                                        IsAllow = fun.IsActive ?? false
                                    }).ToList();

                    foreach (var item in response.Where(t => activePermissions.Contains(t.FunctionalityId)))
                    {
                        item.IsAllow = true;
                    }
                    viewModel.Id = templateDetail.Id;
                    viewModel.TemplateName = templateDetail.TemplateName;
                    viewModel.IsActive = templateDetail.IsActive;
                    viewModel.permissionTemplates = response;
                    return new CustomResponse<PermissionTemplateViewModel>() { IsSuccess = true, Message = "", Data = viewModel };
                }
                else
                {
                    return new CustomResponse<PermissionTemplateViewModel>() { IsSuccess = false, Message = "No Template Found", Data = null };
                }

            }
            catch (Exception ex)
            {
                return new CustomResponse<PermissionTemplateViewModel>() { IsSuccess = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<CustomResponse<PermissionTemplateViewModel>> SavePermissionTemplateMakerRequest(PermissionTemplateViewModel model, string Action)
        {
            try
            {
                List<PermissionTemplateDetail> templateDetails = new List<PermissionTemplateDetail>();
                PermissionTemplate permissionTemplate = new PermissionTemplate
                {
                    Id = model.Id,
                    TemplateName = model.TemplateName,
                    IsActive = model.IsActive,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = DateTime.Now
                };
                foreach (var item in model.permissionTemplates)
                {
                    PermissionTemplateDetail templateDetail = new PermissionTemplateDetail
                    {
                        TemplateId = model.Id,
                        FormName = item.FormName,
                        FunctionalityId = item.FunctionalityId,
                        IsAllow = item.IsAllow
                    };
                    templateDetails.Add(templateDetail);
                }
                permissionTemplate.PermissionTemplateDetails = templateDetails;
                //MakerChecker makerChecker = new MakerChecker();
                //makerChecker.FormId = (Int32)SetUpForm.Permission;
                //makerChecker.MakerStatusId = Convert.ToInt32(model.makerAction);
                //makerChecker.CheckerStatusId = (Int32)CheckerAction.None;
                //makerChecker.MakerId = model.CreatedBy;
                //makerChecker.MakerDate = DateTime.Now;
                //makerChecker.ReferenceId = model.Id;
                //makerChecker.Active = true;
                //makerChecker.OldValueJson = Action == "ADD" ? "" : await GetPermissionMakerRequestById(model.Id);
                //makerChecker.NewValueJson = permissionTemplate != null ? JsonSerializer.Serialize(permissionTemplate) : "";
                //ctx.MakerChecker.Add(makerChecker);
                //await ctx.SaveChangesAsync();
                return new CustomResponse<PermissionTemplateViewModel>() { IsSuccess = true, Message = "", Data = model };
            }
            catch (Exception ex)
            {
                return new CustomResponse<PermissionTemplateViewModel>() { IsSuccess = false, Message = ex.Message, Data = model };
            }
        }
        //public async Task<string> GetPermissionMakerRequestById(int id)
        //{
        //    return await ctx.MakerChecker
        //   .Where(x => (x.ReferenceId == id) && (x.FormId == (int)SetUpForm.Permission))
        //   .Select(item => item.NewValueJson).FirstOrDefaultAsync();
        //}
        public async Task<bool> TemplateNameExistsAsync(string TemplateName)
        {
            return await ctx.PermissionTemplate.AnyAsync(x => x.TemplateName == TemplateName);
        }
        public async Task<ActionResponse> SavePermissionTemplate(PermissionTemplateViewModel model)
        {
            //var dbContextTransaction = ctx.Database.BeginTransaction();
            try
            {
                List<PermissionTemplateDetail> templateDetails = new List<PermissionTemplateDetail>();
                PermissionTemplate permissionTemplate = new PermissionTemplate
                {
                    Id = model.Id,
                    TemplateName = model.TemplateName,
                    IsActive = model.IsActive,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = DateTime.Now
                };
                ctx.PermissionTemplate.Add(permissionTemplate);
                await ctx.SaveChangesAsync();
                model.Id = permissionTemplate.Id;
                foreach (var item in model.permissionTemplates)
                {
                    PermissionTemplateDetail templateDetail = new PermissionTemplateDetail
                    {
                        TemplateId = model.Id,
                        FormName = item.FormName,
                        FunctionalityId = item.FunctionalityId,
                        IsAllow = item.IsAllow
                    };
                    templateDetails.Add(templateDetail);
                }
                ctx.PermissionTemplateDetail.AddRange(templateDetails);
                await ctx.SaveChangesAsync();
                //await dbContextTransaction.CommitAsync();
                return new ActionResponse() { Success = true, ErrorMessage = "" };
            }
            catch (Exception ex)
            {
                return new ActionResponse() { Success = false, ErrorMessage = ex.Message };
            }
        }
        public async Task<ActionResponse> UpdatePermissionTemplate(PermissionTemplateViewModel model)
        {


            if (!model.IsActive)
            {
                string response = CheckUsersExistingRoles(model.Id);
                if (!string.IsNullOrEmpty(response))
                {
                    return new ActionResponse() { Success = false, ErrorMessage = response };
                }
            }


            var dbContextTransaction = ctx.Database.BeginTransaction();
            try
            {
                List<PermissionTemplateDetail> templateDetails = new List<PermissionTemplateDetail>();
                var updatePermissionTemplate = await ctx.PermissionTemplate.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (updatePermissionTemplate != null)
                {
                    updatePermissionTemplate.TemplateName = model.TemplateName;
                    updatePermissionTemplate.IsActive = model.IsActive;
                    updatePermissionTemplate.UpdatedBy = model.UpdatedBy;
                    updatePermissionTemplate.UpdatedDate = DateTime.Now;
                    await ctx.SaveChangesAsync();

                    ctx.PermissionTemplateDetail.RemoveRange(ctx.PermissionTemplateDetail.Where(x => x.TemplateId == model.Id));
                    await ctx.SaveChangesAsync();
                    foreach (var item in model.permissionTemplates)
                    {
                        PermissionTemplateDetail templateDetail = new PermissionTemplateDetail
                        {
                            TemplateId = model.Id,
                            FormName = item.FormName,
                            FunctionalityId = item.FunctionalityId,
                            IsAllow = item.IsAllow
                        };
                        templateDetails.Add(templateDetail);
                    }
                    ctx.PermissionTemplateDetail.AddRange(templateDetails);
                    await ctx.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();
                    return new ActionResponse() { Success = true, ErrorMessage = "" };
                }
                else
                {
                    return new ActionResponse() { Success = false, ErrorMessage = "No Record Found" };
                }

            }
            catch (Exception ex)
            {
                return new ActionResponse() { Success = false, ErrorMessage = ex.Message };
            }




        }
        private string CheckUsersExistingRoles(int RoleId)
        {
            string response = string.Empty;

            var result = ctx.Users.Where(x => x.RoleTemplateId == RoleId).ToList();
            if (result.Count() != 0)
            {
                var users = string.Join(',', result.Select(x => x.FirstName + " " + x.LastName).ToArray());
                response = $"Role Cannot be Inactive, If any user assign to this Role. Assign Users : {users}.";
            }



            return response;



        }

        #region Dispose
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

        #endregion
    }
}
