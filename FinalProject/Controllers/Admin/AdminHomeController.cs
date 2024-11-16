using AutoMapper;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers.Admin
{
    public class AdminHomeController : Controller
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public AdminHomeController(UserService _userService, IMapper _mapper)
        {
            this._userService = _userService;
            this._mapper = _mapper;
        }

        #region AdminHome

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Logout

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("user_id");

            await HttpContext.SignOutAsync();
            return RedirectToAction("SignIn", "Customer");
        }

        #endregion

        #region AdminProfile

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AdminProfile()
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
        public async Task<IActionResult> AdminProfile(UpdatePersonalInformationVM user, IFormFile? userimage)
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

                return RedirectToAction("AdminProfile");
            }

            return RedirectToAction("SignIn");
        }

        #endregion

        #region AdminChangePassword

        [Authorize]
        public async Task<IActionResult> AdminChangePassword()
        {
            return View(new UpdatePasswordVM());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AdminChangePasswordPost(UpdatePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("AdminChangePassword", model);
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
                            return RedirectToAction("AdminProfile", "AdminHome");
                        }
                        else
                        {
                            TempData["FailMessage"] = "Cập nhật mật khẩu thất bại!";
                            return View("AdminChangePassword", model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không chính xác!");
                        return View("AdminChangePassword", model);
                    }
                }
            }

            return RedirectToAction("SignIn");
        }

        #endregion
    }
}
