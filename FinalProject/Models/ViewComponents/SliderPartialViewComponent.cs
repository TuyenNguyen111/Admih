using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Models.ViewComponents
{
    public class SliderPartialViewComponent : ViewComponent
    {
        private readonly RoomService roomService;

        public SliderPartialViewComponent(RoomService roomService)
        {
            this.roomService = roomService;
        }

        public IViewComponentResult Invoke(int postId, string address)
        {
            IEnumerable<RoomPostVM> roomList = roomService.GetRoomListByAddress(address, postId);

            return View(roomList);
        }
    }
}
