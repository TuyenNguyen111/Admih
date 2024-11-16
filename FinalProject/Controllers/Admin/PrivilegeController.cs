using FinalProject.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class PrivilegeController : Controller
    {
        private readonly PrivilegeService _privilegeService;
        public async Task<IActionResult> IndexAsync()
        {
            var privileges = await _privilegeService.GetAllPrivilegesAsync();
            return View();
        }
    }
}
