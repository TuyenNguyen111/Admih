using FinalProject.Services.Admin;
using FinalProject.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace FinalProject.Controllers.Admin
{
    public class ViolationResponseManagementController : Controller
    {
        private readonly ResponseListService responseListService;
        public ViolationResponseManagementController(ResponseListService responseListService) 
        {
            this.responseListService = responseListService;
        }

        [Authorize]
        public IActionResult Index()
        {
            List<ResponseListVM> responseList = responseListService.GetAllResponseList();

            return View(responseList);
        }

        [Authorize]
        public IActionResult ViewResponse(int responseId)
        {
            ResponseListVM response = responseListService.GetResponseById(responseId);

            return View(response);
        }

        [Authorize]
        public IActionResult Approve(int responseId)
        {
            if (responseListService.Approve(responseId))
            {
                TempData["SuccessMessage"] = "Phản hồi đã được duyệt thành công!";
                return RedirectToAction("Index", "ViolationResponseManagement"); 
            }
            else
            {
                TempData["FailMessage"] = "Duyệt phản hồi thất bại!";
                return RedirectToAction("Index", "ViolationResponseManagement"); 
            }
        }

        [Authorize]
        public IActionResult Reject(int responseId)
        {
            try
            {
                if (responseListService.Reject(responseId))
                {
                    TempData["SuccessMessage"] = "Phản hồi đã bị từ chối thành công!";
                }
                else
                {
                    TempData["FailMessage"] = "Từ chối phản hồi thất bại!";
                }

                return RedirectToAction("Index", "ViolationResponseManagement");
            }
            catch (Exception ex)
            {
                TempData["FailMessage"] = "Đã xảy ra lỗi khi từ chối phản hồi.";
                return RedirectToAction("Index", "ViolationResponseManagement");
            }
        }

    }
}
