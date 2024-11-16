using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Controllers
{
    public class FavoriteListController : Controller
    {

        private readonly QlptContext db;
        private readonly FavoriteListService _favoriteListService;

        public FavoriteListController(QlptContext db, FavoriteListService _favoriteListService)
        {
            this.db = db;
            this._favoriteListService = _favoriteListService;
        }

        #region FavoriteListView
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var favoriteListFromDb = await _favoriteListService.GetFavoriteListByUserId(userId);

                return View(favoriteListFromDb);
            }
            else
            {
                var sessionFavoriteList = HttpContext.Session.Get<List<FavoriteListVM>>(MySetting.FAVORITE_KEY) ?? new List<FavoriteListVM>();
                return View(sessionFavoriteList);
            }
        }
        #endregion

        #region AddToFavoriteAuthenticated
        [Authorize]
        public async Task<IActionResult> AddToFavoriteAuthenticated(int id, int quantity = 1)
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                if (await _favoriteListService.CheckExistingFavoriteList(userId))
                {
                    if(await _favoriteListService.CheckExistingPostId(id, userId))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        if (await _favoriteListService.AddFavotiteListPost(id, userId))
                        {
                            TempData["SuccessMessage"] = "Added to favorites successfully!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Failed to add to favorites!";
                        }
                    }
                }
                else
                {
                    if (await _favoriteListService.AddFavoriteList(userId))
                    {
                        if(await _favoriteListService.CheckExistingPostId(id, userId))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            if (await _favoriteListService.AddFavotiteListPost(id, userId))
                            {
                                TempData["SuccessMessage"] = "Added to favorites successfully!";
                            }
                            else
                            {
                                TempData["FailMessage"] = "Failed to add to favorites!";
                            }
                        }
                    }
                    else
                    {
                        TempData["FailMessage"] = "Failed to create favorite list!";
                    }
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region AddToFavoriteAnonymous
        [AllowAnonymous]
        public IActionResult AddToFavoriteAnonymous(int id, int quantity = 1)
        {
            var favorite = HttpContext.Session.Get<List<FavoriteListVM>>(MySetting.FAVORITE_KEY) ?? new List<FavoriteListVM>();

            var item = favorite.SingleOrDefault(p => p.PostId == id);
            if (item == null)
            {
                var result = db.RoomPosts
                    .Where(r => r.PostId == id)
                    .Join(db.RoomImages, r => r.PostId, img => img.PostId, (r, img) => new { r, img })
                    .Join(db.RoomTypes, rimg => rimg.r.RoomTypeId, rt => rt.RoomTypeId, (rimg, rt) => new { rimg.r, rimg.img, rt })
                    .Select(x => new FavoriteListVM
                    {
                        PostId = x.r.PostId,
                        RoomName = x.r.RoomName,
                        RoomImage = x.img.ImageUrl,
                        RoomPrice = (decimal)x.r.RoomPrice,
                        RoomSize = (decimal)x.r.RoomSize,
                        RoomAddress = x.r.Address,
                        totalPost = quantity > 0 ? quantity : 1
                    })
                    .FirstOrDefault();

                if (result != null)
                {
                    favorite.Add(result);
                }
            }

            HttpContext.Session.Set(MySetting.FAVORITE_KEY, favorite);

            return RedirectToAction("Index");
        }

        #endregion


        #region DeleteFavoriteItemAuthenticated
        [Authorize]
        [HttpPost]
        public IActionResult DeleteFavoriteItemAuthenticated(int idPhong)
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                bool result = _favoriteListService.DeleteFavoriteItem(idPhong, userId);

                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể xóa mục yêu thích. Vui lòng thử lại.");
                    return RedirectToAction("Index"); 
                }
            }

            ModelState.AddModelError("", "Bạn cần đăng nhập để thực hiện hành động này.");
            return RedirectToAction("Index");
        }
        #endregion


        #region DeleteFavoriteItemAnonymous
        [AllowAnonymous]
        [HttpPost]
        public IActionResult DeleteFavoriteItemAnonymous(int idPhong)
        {
            var favorite = HttpContext.Session.Get<List<FavoriteListVM>>(MySetting.FAVORITE_KEY) ?? new List<FavoriteListVM>();

            var item = favorite.SingleOrDefault(p => p.PostId == idPhong);

            if (item != null)
            {
                favorite.Remove(item);

                HttpContext.Session.Set(MySetting.FAVORITE_KEY, favorite);
            }

            return RedirectToAction("Index");
        }

        #endregion
    }
}
