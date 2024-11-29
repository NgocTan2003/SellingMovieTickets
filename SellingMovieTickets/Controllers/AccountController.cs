using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Services.Interfaces;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.Accounts;
using SellingMovieTickets.Models.ViewModels.Users;
using SellingMovieTickets.Repository;
using System.Data;
using System.Net.Sockets;
using System.Security.Claims;

namespace SellingMovieTickets.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;


        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IEmailSender emailSender,
             RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _dataContext = dataContext;
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
                    var avatar = findUserName.Avatar ?? "avatar_default.jpg";
                    var roles = await _userManager.GetRolesAsync(findUserName);
                    var role = roles.FirstOrDefault() ?? "Unknown";
                    var email = await _userManager.GetEmailAsync(findUserName);

                    var claims = new List<Claim>
                                {
                                    new Claim(ClaimUserLogin.Id, findUserName.Id),
                                    new Claim(ClaimUserLogin.Avatar, avatar),
                                    new Claim(ClaimUserLogin.UserName, findUserName.FullName ?? findUserName.UserName),
                                    new Claim(ClaimUserLogin.Email, email),
                                    new Claim(ClaimUserLogin.Role, role)
                                };

                    // Tạo claims identity 
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Cookie tồn tại sau khi đóng trình duyệt
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };
                    await _signInManager.SignInWithClaimsAsync(findUserName, authProperties, claims);

                    TempData["Success"] = "Đăng nhập thành công";
                    var receiver = findUserName.Email;
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
                    CustomerManagementModel customerManagement = new CustomerManagementModel
                    {
                        UserId = newUser.Id,
                        CreateDate = DateTime.Now
                    };
                    await _dataContext.CustomerManagements.AddAsync(customerManagement);
                    await _dataContext.SaveChangesAsync();
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");

            //await _signInManager.SignOutAsync();
            //return Redirect(returnUrl);
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailForgotPass(AppUserModel model)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (checkMail == null)
            {
                TempData["Error"] = "Không tìm thấy Email";
                return RedirectToAction("ForgotPassword", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();
                var receiver = checkMail.Email;
                var subject = "Đổi mật khẩu tài khoản";
                var message = $@"
                            <div>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu UMI Cinema của bạn.</div>
                            <a href='{Request.Scheme}://{Request.Host}/Account/ResetPassword?Email={checkMail.Email}&token={token}' 
                               style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                               Đặt lại mật khẩu
                            </a>";
                await _emailSender.SendEmailAsync(receiver, subject, message);
            }

            TempData["Success"] = "Vui lòng kiểm tra Email bạn đã đăng ký tài khoản";
            return RedirectToAction("ForgotPassword", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailChangePass()
        {
            var userEmail = User.FindFirstValue(ClaimUserLogin.Email);
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (checkMail == null)
            {
                TempData["Error"] = "Không tìm thấy Email";
                return RedirectToAction("Index", "CustomerManagement");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();
                var receiver = checkMail.Email;
                var subject = "Đổi mật khẩu tài khoản";
                var message = $@"
                            <div>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu UMI Cinema của bạn.</div>
                            <a href='{Request.Scheme}://{Request.Host}/Account/ResetPassword?Email={checkMail.Email}&token={token}' 
                               style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                               Đổi mật khẩu
                            </a>";


                await _emailSender.SendEmailAsync(receiver, subject, message);
            }

            TempData["Success"] = "Vui lòng kiểm tra Email bạn đã đăng ký tài khoản";
            return RedirectToAction("Index", "CustomerManagement");
        }

        public async Task<IActionResult> ResetPassword(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users.Where(u => u.Email == user.Email)
                .Where(u => u.Token == token).FirstOrDefaultAsync();

            if (checkUser != null)
            {
                ViewBag.Email = checkUser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["Error"] = "Email không tìm thấy hoặc token bị sai";
                return RedirectToAction("ForgotPassword", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResetPassword(ResetPassword user, string token)
        {
            var checkUser = await _userManager.Users.Where(u => u.Email == user.Email).Where(u => u.Token == token).FirstOrDefaultAsync();
            if (checkUser != null)
            {
                string newToken = Guid.NewGuid().ToString();
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkUser, user.Password);
                checkUser.PasswordHash = passwordHash;
                checkUser.Token = newToken;
                await _userManager.UpdateAsync(checkUser);
                TempData["Success"] = "Mật khẩu cập nhật thành công";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Error"] = "Email không tìm thấy hoặc token không đúng";
                return RedirectToAction("ForgotPassword", "Account");
            }
        }

    }
}
