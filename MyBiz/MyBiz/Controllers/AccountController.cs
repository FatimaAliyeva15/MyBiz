using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBiz.DTOs;
using MyBiz_Core.Models;

namespace MyBiz.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> CreateRole()
        {
            IdentityRole role1 = new IdentityRole("Admin");
            IdentityRole role2 = new IdentityRole("Member");

            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);

            return Ok("Rollar yarandi");
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOut");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOut");
            }

            if (!ModelState.IsValid)
                return View();

            var existUserName = await _userManager.FindByNameAsync(registerDTO.UserName);
            var existEmail = await _userManager.FindByEmailAsync(registerDTO.Email);

            if(existUserName != null)
            {
                ModelState.AddModelError("", "cannot be the same username");
                return View();
            }

            if(existEmail != null)
            {
                ModelState.AddModelError("", "cannot be the same email");
                return View();
            }

            AppUser user = new AppUser()
            {
                FullName = registerDTO.Name,
                Email = registerDTO.Email,
                UserName = registerDTO.UserName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOut");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOut");
            }

            if (!ModelState.IsValid)
                return View();

            AppUser user;

            if (loginDTO.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginDTO.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginDTO.UserNameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "UserNameOrEmail or password is not valid");
                return View();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);

            if(result.IsLockedOut)
            {
                ModelState.AddModelError("", "Try again shortly");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserNameOrEmail or password is not correct");
                return View();
            }

            await _signInManager.SignInAsync(user, loginDTO.IsPersistent);

            var role = await _userManager.GetRolesAsync(user);

            //if (role.Contains("Admin"))
            //{
            //    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
