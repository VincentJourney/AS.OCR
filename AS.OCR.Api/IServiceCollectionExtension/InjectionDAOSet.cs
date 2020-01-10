using AS.OCR.Dapper.DAO;
using AS.OCR.IDAO;
using Microsoft.Extensions.DependencyInjection;

namespace AS.OCR.Api.IServiceCollectionExtension
{
    public static class InjectionDAOSet
    {
        public static IServiceCollection AddBusinessDAO(this IServiceCollection services)
        {
            services.AddTransient<IAccountDAO, AccountDAO>();
            services.AddTransient<IApplyPointDAO, ApplyPointDAO>();
            services.AddTransient<IOCRLogDAO, OCRLogDAO>();
            return services;
        }
    }
}
