using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HeyTeam.Web.Services;
using HeyTeam.Identity;
using HeyTeam.Identity.Data;
using HeyTeam.Identity.Seeding;
using HeyTeam.Core.UseCases;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Validation;
using System;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Entities;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.UseCases.Coach;

namespace HeyTeam.Web
{
    public class Startup
    {
        // public Startup(IConfiguration configuration)
        // {
        //     Configuration = configuration;
        // }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(Configuration);
            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("ConnectionString")));
                options.UseSqlServer(Configuration["ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMultitenancy<Club, TenantResolver>();

            services.AddMvc();            
            services.AddScoped<IIdentityInitializer, IdentityInitializer>();
            services.AddScoped<IDbConnectionFactory, ConnectionFactory>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<ISquadRepository, SquadRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IIdentityManager, IdentityManager>();
            services.AddScoped<IValidator<DashboardRequest>, DashboardRequestValidator>();             
            services.AddScoped<IUseCase<DashboardRequest, Response<List<Group>>>, DashboardUseCase>();            
            services.AddScoped<IValidator<AddSquadRequest>, AddSquadRequestValidator>(); 
            services.AddScoped<IUseCase<AddSquadRequest, Response<Guid?>>, AddSquadUseCase>();
            services.AddScoped<IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>>, GetSquadUseCase>();
            services.AddScoped<IValidator<GetSquadRequest>, GetSquadRequestValidator>();
			services.AddScoped<IValidator<AddPlayerRequest>, AddPlayerRequestValidator>();
			services.AddScoped<IUseCase<AddPlayerRequest, Response<Guid?>>, AddPlayerUseCase>();
			services.AddScoped<IValidator<UpdatePlayerRequest>, UpdatePlayerRequestValidator>();
			services.AddScoped<IUseCase<UpdatePlayerRequest, Response<Guid?>>, UpdatePlayerUseCase>();
			services.AddScoped<IValidator<GetPlayerRequest>, GetPlayerRequestValidator>();
			services.AddScoped<IUseCase<GetPlayerRequest, Response<(Player, string)>>, GetPlayerUseCase>();

			services.AddScoped<ICoachRepository, CoachRepository>();
			services.AddScoped<IValidator<AddCoachRequest>, AddCoachRequestValidator>();
			services.AddScoped<IUseCase<AddCoachRequest, Response<Guid?>>, AddCoachUseCase>();

		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public  void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();            
            app.UseMultitenancy<Club>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Dashboard}/{action=Index}/{id?}");
            });
        }
    }
}
