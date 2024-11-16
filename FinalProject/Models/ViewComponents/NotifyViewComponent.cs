using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Models.ViewComponents
{
    public class NotifyViewComponent : ViewComponent
    {
        private readonly RoomFeedbackService _roomFeedbackService;

        public NotifyViewComponent(RoomFeedbackService _roomFeedbackService, UserService _userService)
        {
            this._roomFeedbackService = _roomFeedbackService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = Request.Cookies["user_id"];
            var warningReports = _roomFeedbackService.NotifyUserForViolationPosts(Int32.Parse(userId));
            ViewBag.lstReportFeedback = warningReports;
            return View("~/Views/Shared/Components/Notify/Notification.cshtml", warningReports.Count);
        }
    }
}
