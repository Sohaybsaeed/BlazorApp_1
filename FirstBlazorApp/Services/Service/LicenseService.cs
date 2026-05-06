using FirstBlazorApp.Constants;
using FirstBlazorApp.Data;
using FirstBlazorApp.Models;
using FirstBlazorApp.Models.Custom_Models;
using FirstBlazorApp.Services.IService;
using LicenseIssuer.Console.Models;
using LicenseIssuer.Console.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace FirstBlazorApp.Services.Service
{
    public class LicenseService : ILicenseService
    {
        private readonly EnterpriseOperationsContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly LicenseGenerator _licenseGenerator;
        private readonly IAuthorizationService _authorization;
        private readonly IHttpContextAccessor _http;

        public LicenseService(EnterpriseOperationsContext context, IEncryptionService encryptionService, LicenseGenerator licenseGenerator, IAuthorizationService authorization,
        IHttpContextAccessor http)
        {
            _context = context;
            _encryptionService = encryptionService;
            _licenseGenerator = licenseGenerator;
            _authorization = authorization;
            _http = http;
        }

        public async Task<LicenseDetail?> IssueLicense(LicenseRequest request)
        {
            var user = _http.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var authResult = await _authorization.AuthorizeAsync(
                user,
                null,
                "Permission:IssueLicense");

            if (!authResult.Succeeded)
                throw new UnauthorizedAccessException("You do not have permission to issue licenses.");

            var banks = await GetBanksAsync();
            var bank = banks.FirstOrDefault(b => b.Id == request.SelectedBank);

            if (bank == null)
                throw new Exception(LicenseConstants.BankRequired);

            if (request.GenerateKeyPair && string.IsNullOrWhiteSpace(request.PassPhrase))
            {
                throw new Exception(LicenseConstants.PassPhraseRequired);
            }

            string privateKey = string.Empty;
            string publicKey = string.Empty;

            (privateKey, publicKey) = await LoadExistingLicenseKeys(request, privateKey, publicKey);

            var bankName = bank.BankName;

            LicenseGeneratorRequest licenseGeneratorRequest = GenerateLicenseRequest(request, privateKey, publicKey, bankName);

            var generatedLicense = _licenseGenerator.Generate(licenseGeneratorRequest);

            (LicenseDetail licenseDetail, bool isSaved) = await StoreLicense(request, generatedLicense);

            return isSaved ? licenseDetail : null;
        }

        private async Task<(LicenseDetail licenseDetail, bool isSaved)> StoreLicense(LicenseRequest request, LicenseResponse generatedLicense)
        {
            LicenseDetail licenseDetail = new LicenseDetail()
            {
                LicenseGuid = generatedLicense.LicenseGuid,
                CreatedDate = DateTime.Now,
                ExpiryDate = generatedLicense.ExpiryDate,
                IsActive = true,
                PassPhrase = _encryptionService.Encrypt(request.PassPhrase),
                PrivateKey = generatedLicense.PrivateKey,
                PublicKey = generatedLicense.PublicKey,
                BankId = request.SelectedBank,
                XmlFile = generatedLicense.LicenseXml
            };

            await _context.LicenseDetails.AddAsync(licenseDetail);

            bool isSaved = await _context.SaveChangesAsync() > 0;

            return (licenseDetail, isSaved);
        }

        private async Task<(string privateKey, string publicKey)> LoadExistingLicenseKeys(LicenseRequest request, string privateKey, string publicKey)
        {
            if (!request.GenerateKeyPair)
            {
                var existingLicense = await _context.LicenseDetails
                                                    .Where(x => x.BankId == request.SelectedBank)
                                                    .OrderBy(x => x.CreatedDate)
                                                    .LastOrDefaultAsync();
                
                if (existingLicense is not null)
                {
                    privateKey = existingLicense.PrivateKey;
                    publicKey = existingLicense.PublicKey;
                    request.PassPhrase = _encryptionService.Decrypt(existingLicense.PassPhrase);
                }
                else
                {
                    throw new Exception(LicenseConstants.ExistingLicenseNotFound);
                }
            }

            return (privateKey, publicKey);
        }

        private static LicenseGeneratorRequest GenerateLicenseRequest(LicenseRequest request, string privateKey, string publicKey, string bankName)
        {
            return new LicenseGeneratorRequest()
            {
                BankName = bankName,
                PassPhrase = request.PassPhrase,
                PrivateKey = privateKey,
                PublicKey = publicKey,
                IsKeyPairRequired = request.GenerateKeyPair,
                ExpiryDate = request.ExpiryDate,
            };
        }

        public async Task<List<MasterBankDetail>> GetBanksAsync()
        {
            return await _context.MasterBankDetails
                                 .AsNoTracking()
                                 .Where(b => b.IsActive)
                                 .OrderBy(b => b.BankName)
                                 .Select(b => new MasterBankDetail
                                 {
                                     Id = b.Id,
                                     BankName = b.BankName,
                                     IsActive = true
                                 })
                                 .ToListAsync();
        }

        //public async Task<List<LicenseDetail>> GetAllLicensesAsync()
        //{
        //    return await _context.LicenseDetails
        //        .OrderByDescending(l => l.CreatedDate)
        //        .ToListAsync();
        //}
        public async Task<List<LicenseDetail>> GetAllLicensesAsync()
        {
            var user = _http.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var authResult = await _authorization.AuthorizeAsync(
                user, null, "Permission:ViewLicensesList");

            if (!authResult.Succeeded)
                throw new UnauthorizedAccessException("You do not have permission to view licenses.");

            return await _context.LicenseDetails
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();
        }

    }
}