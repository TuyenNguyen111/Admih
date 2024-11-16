using FinalProject.Data;
using FinalProject.Services.Admin;
using FinalProject.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FinalProject.Controllers.Admin
{
    public class UserManagementController : Controller
    {
        private readonly QlptContext db;
        private readonly UserManagementService userManagementService;

        public UserManagementController(QlptContext db, UserManagementService userManagementService)
        {
            this.db = db;
            this.userManagementService = userManagementService;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 8)
        {

            ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            var pagedUsers = await userManagementService.GetPagedUsersAsync(pageNumber, pageSize);
            return View(pagedUsers);
        }
    
        public async Task<IActionResult> Create()
        {
            ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            return View(new UserManagementVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserManagementVM user)
        {
            if (ModelState.IsValid)
            {
                bool result = await userManagementService.CreateUserAsync(user);
                TempData["SuccessMessage"] = result ? "Thêm người dùng thành công." : "Thêm người dùng thất bại.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserTypes = await db.UserTypes.ToListAsync(); 
            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await userManagementService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserManagementVM userVM)
        {
          

            bool result = await userManagementService.UpdateUserAsync(userVM);
            TempData["SuccessMessage"] = result ? "Cập nhật người dùng thành công." : "Cập nhật người dùng thất bại.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            bool result = await userManagementService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = result ? "Xóa người dùng thành công." : "Xóa người dùng thất bại.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchQuery, int pageNumber = 1, int pageSize = 8)
        {
            ViewBag.SearchQuery = searchQuery;
            var pagedUsers = await userManagementService.SearchUsersAsync(searchQuery, pageNumber, pageSize);
         
            ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            return View("Index", pagedUsers);
        }

        //public async Task<IActionResult> FilterByTypeName(int userTypeId)
        //{
        //    var users = await userManagementService.GetUsersByUserTypeAsync(userTypeId);
        //    ViewBag.UserTypeId = userTypeId;
        //    ViewBag.UserTypes = await db.UserTypes.ToListAsync();
        //    return View("Index", users);
        //}
        public async Task<IActionResult> FilterByTypeName(int userTypeId, int pageNumber = 1, int pageSize = 8)
        {
            var pagedUsers = await userManagementService.GetPagedUsersByUserTypeAsync(userTypeId, pageNumber, pageSize);
            ViewBag.UserTypeId = userTypeId;
            ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            return View("Index", pagedUsers);
        }


    }
}
