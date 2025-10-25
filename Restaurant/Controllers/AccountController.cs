using Applications.Dtos;
using Applications.MenuCategory_servic;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Restaurant.Controllers
{
    public class AccountController:Controller
    {
        private readonly IUserServes _userService;
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(IUserServes _userService,UserManager<ApplicationUser> user)
        {
            this._userService = _userService;
            this._userManager = user;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto userDto)
        {
            if (ModelState.IsValid)
            {
                if (userDto.Password != userDto.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and confirmation password do not match.");
                    return View(userDto);
                }

                var result = await _userService.CreateAsync(userDto);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration completed successfully. Please log in";
                    return RedirectToAction(nameof(Login));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(userDto);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto userDto)
        {
            if (!ModelState.IsValid)
                return View(userDto);

            var result = await _userService.SignInAsync(userDto);

            if (result.Succeeded)
            {
                // نحصل على المستخدم بعد تسجيل الدخول باستخدام البريد الإلكتروني من LoginDto
                var user = await _userManager.FindByNameAsync(userName: userDto.userName);

                // نجلب الأدوار المرتبطة بالمستخدم
                var roles = await _userManager.GetRolesAsync(user);

                // نتحقق من الدور
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("GetAll", "MenuItem");
                }

                // لو مستخدم عادي
                return RedirectToAction("Index", "Home");
            }

            // لو الحساب مقفول
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account locked. Try again later.");
                return View(userDto);
            }

            // لو بيانات خاطئة
            ModelState.AddModelError("", "User name or password is incorrect.");
            return View(userDto);
        }

        public async Task<IActionResult> SignOut()
        {
            await _userService.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}
