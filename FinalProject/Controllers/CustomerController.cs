using AutoMapper;
using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Services;
using FinalProject.Services.Admin;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly PrivilegeService _privilegeService;
        public CustomerController(IMapper mapper, UserService userService, IEmailSender emailSender, PrivilegeService privilegeService)
        {
            _mapper = mapper;
            _userService = userService;
            _emailSender = emailSender;
            _privilegeService = privilegeService;
        }

        #region Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<User>(model);

                bool kq = await _userService.CreateUserAsync(customer, model);

                if (kq)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Error occurred while creating the user. Please try again.");
                }
            }
            return View(model);
        }

        #endregion

        #region SignIn
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginVM login, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            await HttpContext.SignOutAsync();

            if (ModelState.IsValid)
            {
                var customer = await _userService.CheckUserAsync(login);

                if (customer == null)
                {
                    ModelState.AddModelError("Lỗi", "Người dùng không tồn tại!");
                }
                else
                {
                    if (!customer.IsValid)
                    {
                        ModelState.AddModelError("Lỗi", "Tài khoản của bạn đã bị khóa!");
                    }
                    else
                    {
                        if (customer.Password != login.password.ToMd5Hash(customer.RandomKey))
                        {
                            ModelState.AddModelError("Lỗi", "Thông tin đăng nhập không đúng!");
                        }
                        else
                        {
                            var pageAddresses = await _privilegeService.GetPageAddressesForUser(customer.UserTypeId);

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, customer.Email),
                                new Claim(ClaimTypes.Name, customer.Username),
                                new Claim(ClaimTypes.NameIdentifier, customer.UserId.ToString())
                            };

                            foreach (var pageAddress in pageAddresses)
                            {
                                claims.Add(new Claim("PageAccess", pageAddress.Url));
                            }

                            if (customer.UserTypeId == 1)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                            }
                            else if (customer.UserTypeId == 2)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, "Chủ trọ"));
                            }
                            else if (customer.UserTypeId == 3)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, "Khách thuê"));
                            }

                            var claimsIdentity = new ClaimsIdentity(claims, "login");
                            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimPrincipal);

                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddDays(30),
                                HttpOnly = true,
                                IsEssential = true
                            };

                            Response.Cookies.Append("user_id", customer.UserId.ToString(), cookieOptions);

                            if (customer.UserTypeId == 1)
                            {
                                return RedirectToAction("Index", "AdminHome");
                            }
                            else
                            {
                                if (Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    return Redirect("/Home");
                                }
                            }
                        }
                    }
                }
            }
            return View(login);
        }


        #endregion


        #region Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var user = await _userService.getUserById(userId);
                var updatePersonalInfoVM = _mapper.Map<UpdatePersonalInformationVM>(user);
                return View(updatePersonalInfoVM);
            }
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UpdatePersonalInformationVM user, IFormFile? userimage)
        { 
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var _user = await _userService.getUserById(userId);
                if (_user == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    user.userimage = _user.UserImage;
                    return View(user);
                }

                if (userimage != null)
                {
                    var isUpdated = await _userService.UpdateUserImageAsync(_user, userimage);
                    if (isUpdated != null)
                    {
                        user.userimage = isUpdated;
                    } 
                }
                else
                {
                    user.userimage = _user.UserImage;
                }

                _mapper.Map(user, _user); 

                await _userService.UpdateUserInformationAsync(_user, userimage);

                TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";

                return RedirectToAction("Profile"); 
            }

            return RedirectToAction("SignIn");
        }

        #endregion

        #region Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("user_id");

            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion

        #region Change Password

        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            return View(new UpdatePasswordVM()); 
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordPost(UpdatePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var user = await _userService.getUserById(userId);

                if (user != null)
                {
                    if (await _userService.VerifyCurrentPassword(model.currentPassword, user.Password, user.RandomKey))
                    {
                        var isChanged = await _userService.ChangePassword(user, model);

                        if (isChanged)
                        {
                            TempData["SuccessMessage"] = "Cập nhật mật khẩu thành công!";
                            return RedirectToAction("Profile", "Customer");
                        }
                        else
                        {
                            TempData["FailMessage"] = "Cập nhật mật khẩu thất bại!";
                            return View("ChangePassword", model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không chính xác!");
                        return View("ChangePassword", model);
                    }
                }
            }

            return RedirectToAction("SignIn");
        }


        #endregion


        #region Forgot Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM passwordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(passwordVM);
            }

            if (await _userService.EmailExistAsync(passwordVM.Email))
            {
                var token = await _userService.GeneratePasswordResetTokenAsync(passwordVM.Email);

                var resetpwUrl = Url.Action("ResetPassword", "Customer", new { token, email = passwordVM.Email }, Request.Scheme);

                await _emailSender.SendEmailAsync(passwordVM.Email, "Reset Your Password",
                    $"Please reset your password by clicking here: <a href='{resetpwUrl}'>link</a>");

                TempData["Email"] = passwordVM.Email;

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("Email", "Địa chỉ email chưa được đăng ký!");
            return View(passwordVM);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            var email = TempData["Email"]?.ToString();

            ViewBag.Email = email;

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Yêu cầu không hợp lệ.");
                return View("Error"); 
            }

            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            if (await _userService.EmailExistAsync(model.Email))
            {
                var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại.");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion
    }
}
