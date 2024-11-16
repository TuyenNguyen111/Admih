using FinalProject.Data;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Services;
using System.Text.Json;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Controllers
{
    public class RoomFeedbackController : Controller
    {
        private readonly QlptContext _db;
        private readonly RoomFeedbackService _roomFeedbackService;

        public RoomFeedbackController(QlptContext db, RoomFeedbackService roomFeedbackService)
        {
            _db = db;
            _roomFeedbackService = roomFeedbackService;
        }
        public IActionResult Index(int postID)
        {
            ViewBag.PostID = postID;
            ViewBag.Feedbacks = _db.Feedbacks.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Report(int postID, int selectedFeedbackType)
        {
            var userID = Request.Cookies["user_id"];
            string s = await _roomFeedbackService.SendReport(Int32.Parse(userID), postID, selectedFeedbackType);
            TempData["SuccessMessage"] = TempData["SuccessMessage"] = JsonSerializer.Serialize(s); ;
            return RedirectToAction("Detail", "RoomPost", new { id = postID });
        }

        public IActionResult SendResponse(int postId, int feedbackId)
        {
            SendResponseVM sendResponseVM = _roomFeedbackService.GetFeedbackInformation(postId, feedbackId);
            return View(sendResponseVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendResponse(SendResponseVM sendResponse)
        {
            if(ModelState.IsValid)
            {
                bool userHasResponded = _roomFeedbackService.CheckExistingResponse(sendResponse.PostId, sendResponse.FeedbackId);

                if (!userHasResponded)
                {
                    if (_roomFeedbackService.AddNewResponse(sendResponse))
                    {
                        TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công!";
                        return View(sendResponse);
                    }
                    else
                    {
                        TempData["FailMessage"] = "Đã xảy ra lỗi khi gửi phản hồi!";
                        return View(sendResponse);
                    }
                }
                else
                {
                    TempData["FailMessage"] = "Bạn đã gửi phản hồi cho bài viết này rồi!";
                    return View(sendResponse);
                }
            }
            return View(sendResponse);
        }

    }
}
