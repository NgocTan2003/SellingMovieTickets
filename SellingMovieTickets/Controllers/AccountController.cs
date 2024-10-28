using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SellingMovieTickets.Areas.Admin.Services;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.Users;
using System.Data;

namespace SellingMovieTickets.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IEmailSender emailSender,
             RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAccount loginVM)
        {
            if (ModelState.IsValid)
            {
                var findUserName = await _userManager.FindByNameAsync(loginVM.Username);
                if (findUserName == null)
                {
                    TempData["Error"] = "Tài khoản không tồn tại.";
                    return View(loginVM);
                }

                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(loginVM.Username);
                    TempData["Success"] = "Đăng nhập thành công";
                    var receiver = user.Email;
                    var subject = "Đăng nhập trên thiết bị thành công";
                    var message = "Đăng nhập thành công, trải nghiệm dịch vụ nhé";
                    await _emailSender.SendEmailAsync(receiver, subject, message);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Password bị sai";
                    return View(loginVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(loginVM);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterAccount user)
        {
            if (ModelState.IsValid)
            {
                List<string> roles = new List<string>();
                var nameRole = "Customer";

                var findUserName = await _userManager.FindByNameAsync(user.Username);
                var findEmail = await _userManager.FindByEmailAsync(user.Email);
                if (findUserName != null || findEmail != null)
                {
                    TempData["Error"] = "Username hoặc Email đã tồn tại.";
                    return View(user);
                }

                AppUserModel newUser = new AppUserModel
                {
                    UserName = user.Username,
                    Email = user.Email,
                    CreateDate = DateTime.Now
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (!await _roleManager.RoleExistsAsync(nameRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Role.Customer));
                }
                roles.Add(nameRole);
                var AddRole = await _userManager.AddToRolesAsync(newUser, roles);

                if (result.Succeeded && AddRole.Succeeded)
                {
                    TempData["Success"] = "Tạo tài khoản thành công";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Error"] = "Tạo tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    return View(user);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(user);
            }
        }

        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

    }
}
