using Applications.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Applications.MenuCategory_servic
{
    public class UserService: IUserServes
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateAsync(RegisterDto userDto)
        {
            if (userDto == null)
                return IdentityResult.Failed(new IdentityError { Description = "User data is null" });

            ApplicationUser? userApp = await userManager.FindByEmailAsync(userDto.Email);
            if (userApp != null)
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use" });

            var userName = await userManager.FindByNameAsync(userDto.userName);
            if (userName != null)
                return IdentityResult.Failed(new IdentityError { Description = "Username is already in use" });

            //mapping DTO to ApplicationUser
            var applicationUser = new ApplicationUser
            {
                UserName = userDto.userName,
                Email = userDto.Email,
                Address = userDto.Address
            };

            // save user to database
            var result = await userManager.CreateAsync(applicationUser, userDto.Password);
            if (result.Succeeded)
            {
                var role = await userManager.AddToRoleAsync(applicationUser, "Customer");

                await signInManager.SignInAsync(applicationUser, isPersistent: false);
                return IdentityResult.Success;
            }

            var allErrors = result.Errors.Select(e => new IdentityError { Description = e.Description }).ToArray();
            return IdentityResult.Failed(allErrors);

        }

        public async Task<SignInResult> SignInAsync(LoginDto userDto)
        {
            var user = await userManager.FindByNameAsync(userDto.userName)
                ?? await userManager.FindByEmailAsync(userDto.userName);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, userDto.Password,
                    isPersistent: false, lockoutOnFailure: true);
                return result;
            }
            return SignInResult.Failed;
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> CreateRoleAsync(RoleDto role)
        {
            bool exists = await roleManager.RoleExistsAsync(role.roleName);
            if (exists)
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{role.roleName}' already exists" });

            var identityRole = new IdentityRole
            {
                Name = role.roleName,
            };
            var result = await roleManager.CreateAsync(identityRole);
            return result;
        }

       

        public async Task<IdentityResult> AddToRoleAsync(LoginDto userDto, string roleName)
        {
            var user = await userManager.FindByNameAsync(userDto.userName)
                ?? await userManager.FindByEmailAsync(userDto.userName);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!await roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' does not exist" });

            var result = await userManager.AddToRoleAsync(user, roleName);
            return result;
        }
    }
}
