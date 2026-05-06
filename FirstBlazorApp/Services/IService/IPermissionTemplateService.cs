using FirstBlazorApp.Models;
using FirstBlazorApp.Models.Custom_Models;

namespace FirstBlazorApp.Services.IService
{
    public interface IPermissionTemplateService
    {
        #region Add/Update PermissionTemplate

        Task<ActionResponse> SavePermissionTemplate(PermissionTemplateViewModel model);
        Task<ActionResponse> UpdatePermissionTemplate(PermissionTemplateViewModel model);
        #endregion

        #region Get PermissionTemplate
        List<PermissionTemplateDetails> GetAllFunctionalitiesAsync();
        Task<CustomResponse<PermissionTemplateViewModel>> GetPermissionTemplateById(int TemplateId);
        Task<CustomResponse<List<PermissionTemplate>>> GetAllPermissionTemplate();
        Task<CustomResponse<List<PermissionTemplate>>> GetAllActivePermissionTemplate();


        Task<CustomResponse<PermissionTemplateViewModel>> SavePermissionTemplateMakerRequest(PermissionTemplateViewModel model, string Action);




        #endregion


        Task<bool> TemplateNameExistsAsync(string TemplateName);
    }
}
