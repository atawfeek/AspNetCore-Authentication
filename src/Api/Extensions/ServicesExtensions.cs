using AspNetCore.Authentication.Identity.Token.Interfaces;
using AspNetCore.Authentication.Identity.Token.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Authentication.Web.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenIssuer, TokenIssuer>();
        }
    }
}
