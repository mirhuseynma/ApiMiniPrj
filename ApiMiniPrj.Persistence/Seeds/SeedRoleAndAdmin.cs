
namespace ApiMiniPrj.Persistence.Seeds
{
    public static class SeedRoleAndAdmin
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "User" };

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

                if(!adminUser.EmailConfirmed)
                {
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }
            }
        }

    }
}
