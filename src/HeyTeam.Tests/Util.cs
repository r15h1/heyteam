using HeyTeam.Identity;
using HeyTeam.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeyTeam.Tests {
    public static class Util {
        public static IdentityManager GetIdentityInstance(string connectionString) {
            var services = new ServiceCollection();
            services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });
            var serviceProvider = services.BuildServiceProvider();
            var Context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return new IdentityManager(userManager);
        }
    }
}
