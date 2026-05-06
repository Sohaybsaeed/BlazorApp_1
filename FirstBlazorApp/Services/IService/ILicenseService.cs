using FirstBlazorApp.Models;
using FirstBlazorApp.Models.Custom_Models;

namespace FirstBlazorApp.Services.IService
{
    public interface ILicenseService
    {
        Task<LicenseDetail?> IssueLicense(LicenseRequest request);

        Task<List<MasterBankDetail>> GetBanksAsync();

        Task<List<LicenseDetail>> GetAllLicensesAsync();
    }
}