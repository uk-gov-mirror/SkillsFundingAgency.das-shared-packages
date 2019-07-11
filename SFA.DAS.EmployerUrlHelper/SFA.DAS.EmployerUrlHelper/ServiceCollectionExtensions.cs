#if NETCOREAPP
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerUrlHelper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(sp =>
                {
                    var config = sp.GetService<IConfiguration>();
                    var configSection = config.GetSection("SFA.DAS.EmployerUrlHelper");
                    var employerUrlConfig = configSection.Get<EmployerUrlConfiguration>();
                    return employerUrlConfig;
                })
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }
    }
}
#endif