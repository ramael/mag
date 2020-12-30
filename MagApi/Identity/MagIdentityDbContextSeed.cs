using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Identity
{
    public class MagIdentityDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<MagApplicationUser> userManager, RoleManager<MagApplicationRole> roleManager)
        {
            //Seed Roles
            bool wmr = await roleManager.RoleExistsAsync("WarehouseManager");
            if (!wmr)
                await roleManager.CreateAsync(new MagApplicationRole() { Name = "WarehouseManager", Description = "Warehouse Manager: can create, update and delete warehouse related data" });

            bool cmr = await roleManager.RoleExistsAsync("CartManager");
            if (!cmr)
                await roleManager.CreateAsync(new MagApplicationRole() { Name = "CartManager", Description = "Cart Manager: can create, update and delete cart related data" });

            bool ur = await roleManager.RoleExistsAsync("User");
            if (!ur)
                await roleManager.CreateAsync(new MagApplicationRole() { Name = "User", Description = "Generic user: can only read data" });

            //Seed Users
            var user = await userManager.FindByNameAsync("t1");
            if (user == null)
            {
                user = new MagApplicationUser()
                {
                    UserName = "t1",
                    FirstName = "Test1",
                    LastName = "T1LN",
                    Email = "t1@examples.org",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != user.Id))
                {
                    await userManager.CreateAsync(user, "Test123!");
                    await userManager.AddToRolesAsync(user, new string[] { "WarehouseManager", "User" });
                }
            }

             user = await userManager.FindByNameAsync("t2");
            if (user == null)
            {
                user = new MagApplicationUser()
                {
                    UserName = "t2",
                    FirstName = "Test2",
                    LastName = "T2LN",
                    Email = "t2@examples.org",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != user.Id))
                {
                    await userManager.CreateAsync(user, "Test123!");
                    await userManager.AddToRolesAsync(user, new string[] { "CartManager", "User" });
                }
            }

            user = await userManager.FindByNameAsync("t3");
            if (user == null)
            {
                user = new MagApplicationUser()
                {
                    UserName = "t3",
                    FirstName = "Test3",
                    LastName = "T3LN",
                    Email = "t3@examples.org",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != user.Id))
                {
                    await userManager.CreateAsync(user, "Test123!");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}
