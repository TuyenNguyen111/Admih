using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services
{
    public class FavoriteListService
    {
        private readonly QlptContext db;

        public FavoriteListService(QlptContext db)
        {
            this.db = db;
        }

        public async Task<bool> CheckExistingFavoriteList(int user_id)
        {
            try
            {
                var existingFavoriteList = await db.FavoriteLists
                    .FirstOrDefaultAsync(favorite => favorite.UserId == user_id);

                return existingFavoriteList != null; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> AddFavotiteListPost(int id, int userId)
        {
            try
            {
                int favoriteId = GetFavoriteIdByUserId(userId);

                if (favoriteId == 0)
                {
                    return false; 
                }

                var favoritePost = new FavoriteListPost
                {
                    PostId = id,
                    FavoriteId = favoriteId,
                };

                db.FavoriteListPosts.Add(favoritePost);
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private int GetFavoriteIdByUserId(int userId)
        {
            try
            {
                var favoriteId =  db.FavoriteLists
                    .Where(favorite => favorite.UserId == userId)
                    .Select(favorite => favorite.FavoriteListId)
                    .FirstOrDefault();

                return favoriteId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public async Task<bool> AddFavoriteList(int userId)
        {
            try
            {
                var favoriteList = new FavoriteList
                {
                    UserId = userId,
                };

                db.FavoriteLists.Add(favoriteList);
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<List<FavoriteListVM>> GetFavoriteListByUserId(int userId)
        {
            int favoriteId = GetFavoriteIdByUserId(userId);

            var favorites = await (from f in db.FavoriteListPosts
                                   join r in db.RoomPosts on f.PostId equals r.PostId
                                   join img in db.RoomImages on r.PostId equals img.PostId
                                   join rt in db.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
                                   where f.FavoriteId == favoriteId
                                   group img by new
                                   {
                                       r.PostId,
                                       r.RoomName,
                                       r.RoomPrice,
                                       r.RoomSize,
                                       r.Address
                                   } into g
                                   select new FavoriteListVM
                                   {
                                       PostId = g.Key.PostId,
                                       RoomName = g.Key.RoomName,
                                       RoomImage = g.FirstOrDefault().ImageUrl, 
                                       RoomPrice = (decimal)g.Key.RoomPrice,
                                       RoomSize = (decimal)g.Key.RoomSize,
                                       RoomAddress = g.Key.Address
                                   }).ToListAsync();

            return favorites;
        }

        public async Task<bool> CheckExistingPostId(int idPhong, int idUser)
        {
            try
            {
                int favoriteId = GetFavoriteIdByUserId(idUser);

                var favorite_list_post = db.FavoriteListPosts.Where(favorite => favorite.FavoriteId == favoriteId && favorite.PostId == idPhong).FirstOrDefault();

                if (favorite_list_post != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<int> CountNumberOfFavoriteList(int userId)
        {
            try
            {
                int favoriteId = GetFavoriteIdByUserId(userId);

                return await db.FavoriteListPosts
                        .Where(f => f.FavoriteId == favoriteId)
                        .CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        public bool DeleteFavoriteItem(int idPhong, int idUser)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == idUser);

                if (user != null)
                {
                    int favoriteId = GetFavoriteIdByUserId(idUser);

                    var existingFavoritePost = db.FavoriteListPosts.FirstOrDefault(f => f.PostId == idPhong && f.FavoriteId == favoriteId);

                    if (existingFavoritePost != null)
                    {
                        db.FavoriteListPosts.Remove(existingFavoritePost);
                        db.SaveChanges();

                        if (!CheckExistingFavoriteId(favoriteId))
                        {
                            if (DeleteFavoriteListByUser(idUser))
                            {
                                db.SaveChanges();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true; 
                    }
                }
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private bool CheckExistingFavoriteId(int favoriteId)
        {
            try
            {
                return db.FavoriteListPosts.Any(f => f.FavoriteId == favoriteId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool DeleteFavoriteListByUser(int userId)
        {
            try
            {
                var favoriteList = db.FavoriteLists.FirstOrDefault(f => f.UserId == userId);

                if (favoriteList != null)
                {
                    db.FavoriteLists.Remove(favoriteList);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}