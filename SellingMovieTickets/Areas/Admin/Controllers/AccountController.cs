using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.Accounts;
using SellingMovieTickets.Areas.Admin.Models.Users;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.User;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            var userRoles = await (from u in _dataContext.Users
                                   join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                   join r in _dataContext.Roles on ur.RoleId equals r.Id
                                   select new { User = u, Role = r.Name }).ToListAsync();

            IEnumerable<UserViewModel> responseAccounts = userRoles
                                  .GroupBy(ur => ur.User)
                                  .Select(group => new UserViewModel
                                  {
                                      Id = group.Key.Id,
                                      UserName = group.Key.UserName,
                                      Email = group.Key.Email,
                                      FullName = group.Key.FullName,
                                      Gender = group.Key.Gender,
                                      Avatar = group.Key.Avatar,
                                      PhoneNumber = group.Key.PhoneNumber,
                                      CreateBy = group.Key.CreateBy,
                                      CreateDate = group.Key.CreateDate,
                                      ModifiedBy = group.Key.ModifiedBy,
                                      ModifiedDate = group.Key.ModifiedDate.ToString(),
                                      Role = group.Select(x => x.Role).FirstOrDefault()
                                  }).ToList();


            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                responseAccounts = responseAccounts.Where(x =>
                   (x.UserName?.Contains(searchText) ?? false) ||
                   (x.PhoneNumber?.Contains(searchText) ?? false) ||
                   (x.Email?.Contains(searchText) ?? false) ||
                   (x.Role?.Contains(searchText) ?? false));
            }

            int recsCount = responseAccounts.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = responseAccounts.Skip(resSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            var model = new CreateAccount();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccount model)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                var emailEditor = User.FindFirstValue(ClaimUserLogin.Email);

                var findUserName = await _userManager.FindByNameAsync(model.User.Username);
                var findEmail = await _userManager.FindByEmailAsync(model.User.Email);
                if (findUserName != null)
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    TempData["Error"] = "Username đã tồn tại.";
                    return View(model);
                }
                else if (findEmail != null)
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    TempData["Error"] = "Email đã tồn tại.";
                    return View(model);
                }

                var user = new AppUserModel
                {
                    UserName = model.User.Username,
                    Email = model.User.Email,
                    PasswordHash = model.User.Password,
                    PhoneNumber = model.User.PhoneNumber,
                    FullName = model.User.FullName,
                    Gender = model.User.Gender,
                    CreateDate = DateTime.Now,
                    CreateBy = nameEditor,
                    ModifiedDate = DateTime.Now,
                };

                if (model.User.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/accounts");
                    string imageName = Guid.NewGuid().ToString() + "_" + model.User.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await model.User.ImageUpload.CopyToAsync(fs);
                    }
                    user.Avatar = imageName;
                }

                var result = await _userManager.CreateAsync(user, model.User.Password);

                if (result.Succeeded)
                {
                    var selectedRole = model.Role != null ? model.Role : Role.Customer;
                    var role = await _roleManager.FindByIdAsync(selectedRole);

                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else
                    {
                        var roleName = await _roleManager.FindByNameAsync(selectedRole);
                        if (roleName != null)
                        {
                            await _userManager.AddToRoleAsync(user, roleName.Name);
                        }
                    }

                    CustomerManagementModel customerManagement = new CustomerManagementModel
                    {
                        UserId = user.Id,
                        CreateDate = DateTime.Now
                    };
                    await _dataContext.CustomerManagements.AddAsync(customerManagement);
                    await _dataContext.SaveChangesAsync();

                    TempData["Success"] = "Tạo tài khoản thành công.";
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                    return View(model);
                }
            }
            else
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Name");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var model = new UpdateAccount
            {
                User = new UpdateUser
                {
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    Gender = user.Gender,
                    Avatar = user.Avatar
                },
                Role = userRole
            };

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, UpdateAccount model)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var user = await _userManager.FindByIdAsync(Id);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (model.User.ImageUpload == null)
            {
                model.User.Avatar = user.Avatar;
            }

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.UserName = model.User.Username;
                    user.Email = model.User.Email;
                    user.PhoneNumber = model.User.PhoneNumber;
                    user.FullName = model.User.FullName;
                    user.Gender = model.User.Gender;
                    user.ModifiedBy = nameEditor;
                    user.ModifiedDate = DateTime.Now;

                    if (model.User.ImageUpload != null)
                    {
                        if (user.Avatar != null)
                        {
                            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/accounts");
                            string oldFileImage = Path.Combine(uploadsDir, user.Avatar);
                            if (System.IO.File.Exists(oldFileImage))
                            {
                                System.IO.File.Delete(oldFileImage);
                            }
                        }

                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/accounts");
                        string imageName = Guid.NewGuid().ToString() + "_" + model.User.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadDir, imageName);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await model.User.ImageUpload.CopyToAsync(fs);
                        }
                        user.Avatar = imageName;
                    }
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        if (model.Role != null)
                        {
                            var currentRoles = await _userManager.GetRolesAsync(user);
                            var selectedRole = model.Role;
                            // xóa vai trò hiện tại
                            if (currentRoles.Any())
                            {
                                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            }

                            var role = await _roleManager.FindByNameAsync(selectedRole);
                            if (role != null)
                            {
                                await _userManager.AddToRoleAsync(user, role.Name);
                            }
                        }

                        TempData["Success"] = "Cập nhật tài khoản thành công.";
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        TempData["Error"] = "Cập nhật tài khoản không thành công.";
                        model.Role = userRole;
                        return View(model);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Name", "Name");
                    model.Role = userRole;
                    return View(model);
                }
            }
            else
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "Name", "Name");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                model.Role = userRole;
                return View(model);
            }
        }


        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Avatar != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/accounts");
                string oldFileImage = Path.Combine(uploadsDir, user.Avatar);
                if (System.IO.File.Exists(oldFileImage))
                {
                    System.IO.File.Delete(oldFileImage);
                }
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["Success"] = "User đã được xóa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _dataContext.Users.Find(item);
                        if (obj.Avatar != null)
                        {
                            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/accounts");
                            string oldFileImage = Path.Combine(uploadsDir, obj.Avatar);
                            if (System.IO.File.Exists(oldFileImage))
                            {
                                System.IO.File.Delete(oldFileImage);
                            }
                        }
                        _dataContext.Users.Remove(obj);
                        _dataContext.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
