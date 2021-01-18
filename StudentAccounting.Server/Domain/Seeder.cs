using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentAccounting.Server.Domain.Entities;
using System;
using System.Threading.Tasks;
using Bogus;
using StudentAccounting.Server.Constants;
using System.Collections.Generic;


namespace StudentAccounting.Server.Domain
{
    public static class Seeder
    {
        #region SetTestAdmin
        public static async Task SetTestAdmin(
           IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var user = GetTestAdmin(configuration);
            var password = configuration["StudyDate:AdminUser:Password"];
            var role = Roles.ADMIN;

            await SetTestUser(serviceProvider, user, password, role);
        }
        private static DbUser GetTestAdmin(IConfiguration configuration)
            => new DbUser
            {
                UserName = configuration["StudyDate:AdminUser:UserName"],
                Email = configuration["StudyDate:AdminUser:Email"],
                EmailConfirmed = true
            };
        #endregion

        #region SetTestUsers
        public static async Task SetTestUsers(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userList = GetTestUser(100);
            var password = configuration["StudyDate:User:Password"];
            var role = Roles.STUDENT;

            foreach (var user in userList)
            {
                await SetTestUser(serviceProvider, user, password, role);
            }

        }

        private static List<DbUser> GetTestUser(int count = 1)
        {
            var isEmailConfirmed = true;
            var generator = new Faker<DbUser>()
                            .RuleFor(user => user.FirstName, faker => faker.Name.FirstName())
                            .RuleFor(user => user.LastName, faker => faker.Name.LastName())
                            .RuleFor(user => user.Age, faker => faker.Random.Int(10, 120))
                            .RuleFor(user => user.UserName, faker => faker.Internet.UserName())
                            .RuleFor(user => user.Email, faker => faker.Internet.Email())
                            .RuleFor(user => user.EmailConfirmed, faker => isEmailConfirmed)
                            .RuleFor(user => user.RegisteredDate, faker => DateTime.UtcNow);
            var testUser = generator.Generate(count);
            return testUser;
        }
        #endregion

        private static async Task SetTestUser(
           IServiceProvider serviceProvider, DbUser user, string password, string role)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<DbUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await userManager.FindByEmailAsync(user.UserName) == null)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}