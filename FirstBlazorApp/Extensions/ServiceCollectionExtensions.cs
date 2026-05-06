using FirstBlazorApp.Data;
using Microsoft.EntityFrameworkCore;
using FirstBlazorApp.Services.Service;

namespace FirstBlazorApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var webConn = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(webConn))
                throw new Exception("Encrypted connection string is missing (ConnectionStrings:DefaultEncrypted).");

            var enterpriseConn = configuration.GetConnectionString("EnterpriseConnection");
            
            if (string.IsNullOrWhiteSpace(enterpriseConn))
                throw new Exception("Encrypted connection string is missing (ConnectionStrings:DefaultEncrypted).");

            var encryptionService = new EncryptionService(configuration);

            var decryptedWebConn = encryptionService.Decrypt(webConn);
            if (string.IsNullOrWhiteSpace(decryptedWebConn))
                throw new Exception("Decrypted connection string is empty. Check EncryptionSettings:Key and encrypted value.");


            var decryptedEnterpriseConn = encryptionService.Decrypt(enterpriseConn);
            if (string.IsNullOrWhiteSpace(decryptedEnterpriseConn))
                throw new Exception("Decrypted connection string is empty. Check EncryptionSettings:Key and encrypted value.");

            services.AddDbContext<IBRetailDbContext>(options => options.UseSqlServer(decryptedWebConn));
            services.AddDbContext<EnterpriseOperationsContext>(options => options.UseSqlServer(decryptedEnterpriseConn));

            return services;
        }
    }
}