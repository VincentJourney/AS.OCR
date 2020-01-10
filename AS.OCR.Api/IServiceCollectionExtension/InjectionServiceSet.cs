using AS.OCR.IService;
using AS.OCR.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Api.IServiceCollectionExtension
{
    public static class InjectionServiceSet
    {
        public static IServiceCollection AddBusinessService(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            return services;
        }
    }
}
