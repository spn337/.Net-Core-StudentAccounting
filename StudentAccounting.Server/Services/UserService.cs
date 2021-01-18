using Microsoft.AspNetCore.Identity;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Domain;
using StudentAccounting.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace StudentAccounting.Server.Services
{
    public interface IUserService
    {
        IQueryable<DbUser> Users { get; }
        DbUser GetUserById(string id);
        DbUser GetUserByEmail(string email);
        IdentityResult CreateUser(DbUser user, string password);
        IdentityResult RemoveUser(DbUser user);
        IdentityResult UpdateUser(DbUser user);
    }


    public class UserService : IUserService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public UserService(
            UserManager<DbUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IQueryable<DbUser> Users { get => userManager.Users; }

        public DbUser GetUserById(string id)
            => userManager.Users.FirstOrDefault(user => user.Id == id);
        public DbUser GetUserByEmail(string email)
            => userManager.Users.FirstOrDefault(user => user.Email == email);


        public IdentityResult CreateUser(DbUser user, string password)
            => CreateUserAsync(user, password).Result;

        public IdentityResult RemoveUser(DbUser user)
           => RemoveUserAsync(user).Result;

        public IdentityResult UpdateUser(DbUser user)
            => UpdateUserAsync(user).Result;




        private async Task<IdentityResult> CreateUserAsync(DbUser user, string password)
        {
            var createUserResult = await userManager.CreateAsync(user, password);

            if (createUserResult.Succeeded)
            {
                await SetUserRoleAsync(user);
            }

            return createUserResult;
        }

        private async Task SetUserRoleAsync(DbUser user)
        {
            var role = await roleManager.FindByNameAsync(Roles.STUDENT);

            if (role == null)
            {
                role = new IdentityRole { Name = Roles.STUDENT };
                await roleManager.CreateAsync(role);
            }

            await userManager.AddToRoleAsync(user, role.Name);
        }

        private async Task<IdentityResult> RemoveUserAsync(DbUser user)
            => await userManager.DeleteAsync(user);

        private async Task<IdentityResult> UpdateUserAsync(DbUser user)
            => await userManager.UpdateAsync(user);
    }
}
