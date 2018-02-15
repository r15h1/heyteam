using HeyTeam.Core;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Models;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

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
			services.Configure<FileConfiguration>(Configuration);
			services.Configure<CryptographicConfiguration>(Configuration);
			services.Configure<SmtpConfiguration>(Configuration);

			services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("ConnectionString")));
                options.UseSqlServer(Configuration["ConnectionString"]));

			services.AddIdentity<ApplicationUser, IdentityRole>(config => {
				config.SignIn.RequireConfirmedEmail = true;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			 services.ConfigureApplicationCookie(options =>
			 {
				 options.LoginPath = "/accounts/login";
				 options.LogoutPath = "/accounts/logout";
				 options.AccessDeniedPath = "/accounts/accessdenied";
			 }); 

			services.AddMemoryCache();
			// Add application services.
			services.AddTransient<IEmailSender, EmailSender>();
            services.AddMultitenancy<Club, TenantResolver>();			

			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddMvc();

			services.AddDataProtection()
				.PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\Keys"))
				.SetApplicationName("HeyTeam.Web");

			services.AddAuthorization(options =>
			{
				options.AddPolicy("Administrator", policy => policy.Requirements.Add(new RolesRequirement(new string[] { "Administrator" })));

				options.AddPolicy("Player", policy => policy.Requirements.Add(new RolesRequirement(new string[] { "Player" })));
				options.AddPolicy("Player", policy => policy.Requirements.Add(new MembershipRequirement("Player")));

				options.AddPolicy("Coach", policy => policy.Requirements.Add(new RolesRequirement(new string[] { "Coach" })));
				options.AddPolicy("Coach", policy => policy.Requirements.Add(new MembershipRequirement("Coach")));				
			});

			services.AddSingleton<IAuthorizationHandler, RolesHandler>();
			services.AddScoped<IAuthorizationHandler, MembershipRequirementHandler>();

			services.AddScoped<IIdentityInitializer, IdentityInitializer>();
			services.AddScoped<IIdentityManager, IdentityManager>();
			services.AddScoped<IIdentityQuery, IdentityQuery>();
			services.AddScoped<IDbConnectionFactory, ConnectionFactory>();

			services.AddScoped<IClubQuery, ClubQuery>();
			services.AddScoped<IClubRepository, ClubRepository>();
			services.AddScoped<ISquadQuery, SquadQuery>();
			services.AddScoped<ISquadRepository, SquadRepository>();
			services.AddScoped<ISquadService, SquadService>();            

			services.AddScoped<IDashboardQuery, DashboardQuery>();
			services.AddScoped<IValidator<DashboardRequest>, DashboardRequestValidator>();
			
			services.AddScoped<IValidator<PlayerRequest>, PlayerRequestValidator>();
			services.AddScoped<IMemberQuery, MemberQuery>();
			services.AddScoped<IPlayerRepository, PlayerRepository>();
			services.AddScoped<IPlayerService, PlayerService>();

			services.AddScoped<ICoachRepository, CoachRepository>();
			services.AddScoped<IValidator<CoachRequest>, CoachRequestValidator>();
			services.AddScoped<ICoachService, CoachService>();

			services.AddScoped<IValidator<NewEventReviewRequest>, NewEventReviewRequestValidator>();
			services.AddScoped<IValidator<EventSetupRequest>, EventSetupRequestValidator>();
			services.AddScoped<IValidator<EventDeleteRequest>, EventDeleteRequestValidator>();
			services.AddScoped<IEventQuery, EventQuery>();
			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IValidator<EventAttendanceRequest>, EventAttendanceRequestValidator>();

			services.AddScoped<IFileHandlerFactory, FileHandlerFactory>();
			services.AddScoped<IValidator<TrainingMaterialRequest>, TrainingMaterialRequestValidator>();
			services.AddScoped<IValidator<ReSyncRequest>, TrainingMaterialReSyncRequestValidator>();
			services.AddScoped<ILibraryRepository, LibraryRepository>();
			services.AddScoped<ILibraryService, LibraryService>();
			services.AddScoped<ILibraryQuery, LibraryQuery>();

			services.AddScoped<IValidator<AccountRequest>, AccountRequestValidator>();
			services.AddScoped<IAccountsService, AccountsService>();

			services.AddScoped<IAvailabilityQuery, AvailabilityQuery>();
			services.AddScoped<IValidator<NewAvailabilityRequest>, NewAvailabilityRequestValidator>();
			services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
			services.AddScoped<IAvailabilityService, AvailabilityService>();

			services.AddScoped<IValidator<EmailReportRequest>, EmailReportRequestValidator>();
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
                   name: "area",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

				//routes.MapRoute(
				//	name: "membership",
				//	template: "{area:exists=membership}/{memberid:guid}/{controller=Home}/{action=Index}");

				routes.MapRoute(
                    name: "default",
                    template: "{controller=AreaSelectionController}/{action=Index}");
            });
        }
    }
}