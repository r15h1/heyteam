using HeyTeam.Core;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Identity;
using HeyTeam.Identity.Data;
using HeyTeam.Identity.Seeding;
using HeyTeam.Lib.Data;
using HeyTeam.Lib.Queries;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Services;
using HeyTeam.Lib.Settings;
using HeyTeam.Lib.Validation;
using HeyTeam.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeyTeam.Web {
	public class Startup
    {
        // public Startup(IConfiguration configuration)
        // {
        //     Configuration = configuration;
        // }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
			builder.AddJsonFile("appsettings.json");
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
			services.Configure<VideoConfiguration>(Configuration);

			services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("ConnectionString")));
                options.UseSqlServer(Configuration["ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMultitenancy<Club, TenantResolver>();

			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddMvc();   
			
            services.AddScoped<IIdentityInitializer, IdentityInitializer>();
			services.AddScoped<IIdentityManager, IdentityManager>();
			services.AddScoped<IDbConnectionFactory, ConnectionFactory>();

			services.AddScoped<IClubQuery, ClubQuery>();
			services.AddScoped<IClubRepository, ClubRepository>();
			services.AddScoped<ISquadQuery, SquadQuery>();
			services.AddScoped<ISquadRepository, SquadRepository>();
			services.AddScoped<ISquadService, SquadService>();            

			services.AddScoped<IDashboardQuery, DashboardQuery>();
			services.AddScoped<IValidator<DashboardRequest>, DashboardRequestValidator>();
			
			services.AddScoped<IValidator<PlayerRequest>, PlayerRequestValidator>();
			services.AddScoped<IPlayerQuery, PlayerQuery>();
			services.AddScoped<IPlayerRepository, PlayerRepository>();
			services.AddScoped<IPlayerService, PlayerService>();

			services.AddScoped<ICoachQuery, CoachQuery>();
			services.AddScoped<ICoachRepository, CoachRepository>();
			services.AddScoped<IValidator<CoachRequest>, CoachRequestValidator>();
			services.AddScoped<ICoachService, CoachService>();

			services.AddScoped<IValidator<EventSetupRequest>, EventSetupRequestValidator>();
			services.AddScoped<IValidator<EventDeleteRequest>, EventDeleteRequestValidator>();
			services.AddScoped<IEventQuery, EventQuery>();
			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IEventService, EventService>();

			services.AddScoped<IFileHandlerFactory, FileHandlerFactory>();
			services.AddScoped<IValidator<TrainingMaterialRequest>, TrainingMaterialRequestValidator>();
			services.AddScoped<IValidator<ReSyncRequest>, TrainingMaterialReSyncRequestValidator>();
			services.AddScoped<ILibraryRepository, LibraryRepository>();
			services.AddScoped<ILibraryService, LibraryService>();
			services.AddScoped<ILibraryQuery, LibraryQuery>();

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