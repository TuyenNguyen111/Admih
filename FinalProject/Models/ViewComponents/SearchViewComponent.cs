using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Models.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {
        private readonly QlptContext _db;

        public SearchViewComponent(QlptContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var data = _db.RoomTypes.Select(type => new RoomTypeMenuVM
            {
                RoomTypeId = type.RoomTypeId,
                TypeName = type.TypeName,
                Quantity = type.RoomPosts.Count
            }).OrderBy(x => x.TypeName);

            return View(data);
        }
    }
}
