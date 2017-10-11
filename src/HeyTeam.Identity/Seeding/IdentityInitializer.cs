using System;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace HeyTeam.Identity.Seeding {
    public class IdentityInitializer : IIdentityInitializer {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //This example just creates an Administrator role and one Admin users
        public async Task Initialize() {
            //create database schema if none exists
            _context.Database.EnsureCreated();

            //If there is already an Administrator role, abort
            if (_context.Roles.Any(r => r.Name == "Administrator")) return;

            //Create the Administartor Role
            await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            await _roleManager.CreateAsync(new IdentityRole("Player"));
            await _roleManager.CreateAsync(new IdentityRole("Coach"));

            //Create the default Admin account and apply the Administrator role
            string user = "admin@heyteam.com";
            string password = "1#22$Ch@1geThi5";
            var result =  await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, EmailConfirmed = true}, password);
            Console.Write(result.Succeeded);
            Console.Write(result.Errors.Select(r => r.Description).FirstOrDefault());
            var roleresult = await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
            Console.Write(roleresult.Succeeded);
            Console.Write(roleresult.Errors.Select(r => r.Description).FirstOrDefault());
        }
    }
}