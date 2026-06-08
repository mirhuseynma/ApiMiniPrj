
using ApiMiniPrj.Domain.Constants;

namespace ApiMiniPrj.Persistence.Seeds
{
    public static class SeedRoleAndAdmin
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "User", "Organizer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }

            var adminEmail = "admin@code.edu.az";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = ""
                };
                var createAdminResult = await userManager.CreateAsync(newAdminUser, "Admin123!");
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                if (!adminUser.EmailConfirmed)
                {
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }
            }

            if (adminUser  != null)
            {
                if(!adminUser.EmailConfirmed)
                {
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }
            }

            await SeedRolesAndAdminAsync(roleManager, "Admin", Permissions.All());
            await SeedRolesAndAdminAsync(roleManager, "User", [Permissions.Events.View, Permissions.Tickets.View]);
            await SeedRolesAndAdminAsync(roleManager, "Organizer", 
                [
                    Permissions.Events.View,
                    Permissions.Tickets.View,
                    Permissions.Events.Create,
                    Permissions.Events.Delete,
                    Permissions.Events.Edit,
                    Permissions.Tickets.Create,
                    Permissions.Tickets.Delete,
                    Permissions.Tickets.Edit,
                    Permissions.Events.AddBanner,

                    Permissions.Organizers.View
                ]);

        }

        private static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, string roleName, IEnumerable<string> permissions)
        { 
            var roleExists = await roleManager.FindByNameAsync(roleName);
            if (roleExists == null) return;
            var existingClaims = await roleManager.GetClaimsAsync(roleExists);
            foreach (var permission in permissions)
            {
                bool hasPermission = existingClaims.Any(c => c.Type == "Permission" && c.Value == permission);
                if (!hasPermission) await roleManager.AddClaimAsync(roleExists, new Claim("Permission", permission));
            }

        }

    }
}
