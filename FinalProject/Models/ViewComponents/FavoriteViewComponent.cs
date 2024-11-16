using FinalProject.Helpers;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Models.ViewComponents
{
    public class FavoriteViewComponent : ViewComponent
    {
        private readonly FavoriteListService _favoriteListService;
        private readonly UserService _userService;

        public FavoriteViewComponent(FavoriteListService _favoriteListService, UserService _userService)
        {
            this._userService = _userService;
            this._favoriteListService = _favoriteListService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int quantity;

            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var user = await _userService.getUserById(userId);
                if(user != null)
                {
                    int favoriteList = await _favoriteListService.CountNumberOfFavoriteList(userId);
                    quantity = favoriteList;
                }    
                else
                {
                    quantity = 0;
                }    
            }
            else
            {
                var favorite = HttpContext.Session.Get<List<FavoriteListVM>>(MySetting.FAVORITE_KEY) ?? new List<FavoriteListVM>();
                quantity = favorite.Sum(p => p.totalPost);
            }    

            return View("FavoritePanel", quantity);
        }
    }
}
