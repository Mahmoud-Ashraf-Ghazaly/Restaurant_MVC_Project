using Applications.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuCategory_servic
{
    public interface IUserServes
    {
        public Task<IdentityResult> CreateAsync(RegisterDto userDto);
        public Task<SignInResult> SignInAsync(LoginDto userDto);
        public Task SignOutAsync();

       
        public Task<IdentityResult> CreateRoleAsync(RoleDto role);
       
        public Task<IdentityResult> AddToRoleAsync(LoginDto userDto, string roleName);

    }
}
