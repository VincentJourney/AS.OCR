using AS.OCR.Dapper;
using AS.OCR.IDAO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Api.IServiceCollectionExtension
{
    public static class InjectionDAOSet
    {
        public static IServiceCollection AddBusinessDAO(this IServiceCollection services)
        {
            services.AddTransient<IAccountDAO, AccountDAO>();
            return services;
        }
    }
}
