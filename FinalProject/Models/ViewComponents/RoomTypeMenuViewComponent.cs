using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace FinalProject.Models.ViewComponents
{
    public class RoomTypeMenuViewComponent : ViewComponent
    {
        private readonly QlptContext db;

        public RoomTypeMenuViewComponent(QlptContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var data = db.RoomTypes.Select(type => new RoomTypeMenuVM
            {
                RoomTypeId = type.RoomTypeId,
                TypeName = type.TypeName,
                Quantity = type.RoomPosts.Count
            }
            ).OrderBy(p => p.TypeName);

            return View(data);
        }

    }
}
