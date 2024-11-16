using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace FinalProject.Services
{
    public class RoomService
    {
        private readonly QlptContext db;
        public RoomService(QlptContext db)
        {
            this.db = db;
        }

        public IEnumerable<RoomTypeMenuVM> GetRoomType()
        {
            try
            {
                var data = db.RoomTypes.Select(type => new RoomTypeMenuVM
                {
                    RoomTypeId = type.RoomTypeId,
                    TypeName = type.TypeName,
                }).OrderBy(x => x.TypeName).ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<RoomTypeMenuVM>(); ;
            }
        }

        public bool CheckExistingCoordinate(double latitude, double longitude)
        {
            try
            {
                var existingRoomCoordinate = db.RoomCoordinates.FirstOrDefault(coor => coor.Latitude == latitude && coor.Longitude == longitude);

                if (existingRoomCoordinate != null)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<int> GetRoomCoordinateId(double latitude, double longitude)
        {
            try
            {
                var coordinate = await db.RoomCoordinates
                    .FirstOrDefaultAsync(coor => coor.Latitude == latitude && coor.Longitude == longitude);

                if (coordinate != null)
                {
                    return coordinate.RoomCoordinateId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRoomCoordinateId: {ex.Message}");
                return 0;
            }
        }

        private string GetRoomTypeName(int roomTypeId)
        {
            try
            {
                string typeName = db.RoomTypes.Where(type => type.RoomTypeId == roomTypeId).Select(type => type.TypeName).SingleOrDefault(); 

                return typeName ?? string.Empty; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty; 
            }
        }

        private string ExtractDistrictFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return string.Empty;
            }

            var parts = address.Split(',');

            if (parts.Length >= 2)
            {
                var districtPart = parts[^1].Trim(); 

                if (districtPart.StartsWith("Quận", StringComparison.OrdinalIgnoreCase))
                {
                    return districtPart; 
                }
            }
            return string.Empty;
        }


        public int? AddNewRoomPost(RoomPostContentVM roomPostContentVM, int userId)
        {
            try
            {
                var roomTypeName = GetRoomTypeName(roomPostContentVM.RoomTypeId);
                if (string.IsNullOrEmpty(roomTypeName))
                {
                    throw new Exception("Room type name is null or empty.");
                }

                var district = ExtractDistrictFromAddress(roomPostContentVM.RoomAddress);
                if (string.IsNullOrEmpty(district))
                {
                    throw new Exception("District is null or empty.");
                }

                var roomPost = new RoomPost
                {
                    Quantity = roomPostContentVM.Quantity,
                    RoomCoordinateId = roomPostContentVM.RoomCoordinateId,
                    RoomPrice = roomPostContentVM.RoomPrice,
                    RoomSize = roomPostContentVM.RoomSize,
                    Address = roomPostContentVM.RoomAddress,
                    RoomDescription = roomPostContentVM.RoomDescription,
                    DatePosted = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(30),
                    StatusId = 1,
                    UserId = userId,
                    RoomTypeId = roomPostContentVM.RoomTypeId,
                    RoomName = $"{roomTypeName} {district}"
                };

                db.RoomPosts.Add(roomPost);
                db.SaveChanges();

                return roomPost.PostId; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public int AddNewRoomCoordinate(double latitude, double longitude)
        {
            try
            {
                var roomCoordinate = new RoomCoordinates
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                db.RoomCoordinates.Add(roomCoordinate);

                db.SaveChanges();

                return roomCoordinate.RoomCoordinateId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
        }





        public IEnumerable<RoomPostVM> GetRoomListByAddress(string address, int postId)
        {
            try
            {
                string district = ExtractDistrictFromAddress(address);

                if (string.IsNullOrEmpty(district))
                {
                    return Enumerable.Empty<RoomPostVM>();
                }
                else
                {
                    var roomList = (from rp in db.RoomPosts
                                    join ri in db.RoomImages on rp.PostId equals ri.PostId
                                    join rt in db.RoomTypes on rp.RoomTypeId equals rt.RoomTypeId
                                    join rs in db.RoomStatuses on rp.StatusId equals rs.RoomStatusId
                                    where rs.RoomStatusId == 1
                                          && !string.IsNullOrEmpty(rp.Address)
                                          && rp.PostId != postId 
                                    select new
                                    {
                                        rp,
                                        ri,
                                        rt
                                    }).ToList() 
                .Where(x => ExtractDistrictFromAddress(x.rp.Address) == district) 
                .Select(x => new RoomPostVM
                {
                    PostId = x.rp.PostId,
                    RoomName = x.rp.RoomName,
                    Quantity = (int)x.rp.Quantity,
                    RoomDescription = x.rp.RoomDescription,
                    RoomImage = x.ri.ImageUrl,
                    RoomPrice = (decimal)x.rp.RoomPrice,
                    RoomSize = (decimal)x.rp.RoomSize,
                    RoomAddress = x.rp.Address.Replace(
                        x.rp.Address.Substring(
                            x.rp.Address.IndexOf(',') + 1,
                            x.rp.Address.IndexOf(',', x.rp.Address.IndexOf(',') + 1) - x.rp.Address.IndexOf(',')
                        ),
                        string.Empty),
                    RoomType = x.rt.TypeName
                }).ToList();

                    return roomList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<RoomPostVM>();
            }
        }

        public List<UtilityVM> GetUtilities()
        {
            try
            {
                var data = db.Utilities.Select(utility => new UtilityVM
                {
                    utilityId = utility.UtilityId,
                    utilityName = utility.UtilityName,
                    description = utility.Description,
                }).OrderBy(x => x.utilityName).ToList();

                return data;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UtilityVM>();
            }
        }

        public void SaveSelectedUtilities(int postId, List<int?> selectedUtilities)
        {
            try
            {
                foreach (var item in selectedUtilities)
                {
                    var data = new RoomUtility
                    {
                        PostId = postId,
                        UtilityId = item
                    };

                    db.RoomUtilities.Add(data);
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SaveRoomImage(int newRoomId, string uniqueFileName)
        {
            try
            {
                var roomImage = new RoomImage
                {
                    PostId = newRoomId,
                    ImageUrl = uniqueFileName
                };

                db.RoomImages.Add(roomImage);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public IEnumerable<RoomManagementVM> GetRoomListByUserId(int userId)
        {
            try
            {
                var roomList = from r in db.RoomPosts
                               join img in db.RoomImages on r.PostId equals img.PostId into imgGroup
                               where r.UserId == userId
                               select new RoomManagementVM
                               {
                                   PostId = r.PostId,
                                   RoomPrice = (decimal)r.RoomPrice,
                                   RoomAddress = r.Address,
                                   RoomImage = imgGroup.FirstOrDefault().ImageUrl,
                                   RoomName = r.RoomName,
                                   RoomSize = (decimal)r.RoomSize
                               };
                return roomList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<RoomManagementVM>();
            }
        }

        public RoomPostContentVM GetRoomPostContent(int postId)
        {
            try
            {
                var roomPost = (from room in db.RoomPosts
                               where room.PostId == postId
                               select new RoomPostContentVM
                               {
                                   PostId = room.PostId,
                                   RoomAddress = room.Address,
                                   RoomCoordinateId = (int)room.RoomCoordinateId,
                                   RoomDescription = room.RoomDescription,
                                   RoomName = room.RoomName,
                                   RoomPrice = (decimal)room.RoomPrice,
                                   RoomSize = (decimal)room.RoomSize,
                                   RoomTypeId = (int)room.RoomTypeId,
                                   Quantity = (int)room.Quantity,
                                   StatusId = (int)room.StatusId,
                                   UserId = (int)room.UserId,
                               }).FirstOrDefault();

                return roomPost;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<int?> GetUtilitiesByPostId(int postId)
        {
            try
            {
                var utilitiesList = (from u in db.RoomUtilities
                                     where u.PostId == postId
                                     select u.UtilityId).ToList();

                return utilitiesList.Any() ? utilitiesList : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<string> GetImagesByPostId(int postId)
        {
            try
            {
                var imageList = (from img in db.RoomImages
                                 where img.PostId == postId
                                 select img.ImageUrl).ToList();

                return imageList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> CheckRoomAddresAsync(string address)
        {
            try
            {
                var post = await db.RoomPosts.FirstOrDefaultAsync(post => post.Address == address);
                return post != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateRoomPostAsync(RoomPostContentVM room)
        {
            try
            {
                var roomTypeName = GetRoomTypeName(room.RoomTypeId);
                if (string.IsNullOrEmpty(roomTypeName))
                {
                    throw new Exception("Room type name is null or empty.");
                }

                var district = ExtractDistrictFromAddress(room.RoomAddress);
                if (string.IsNullOrEmpty(district))
                {
                    throw new Exception("District is null or empty.");
                }

                var existingRoomPost = await db.RoomPosts.FirstOrDefaultAsync(r => r.PostId == room.PostId);
                if (existingRoomPost != null)
                {
                    existingRoomPost.Quantity = room.Quantity;
                    existingRoomPost.RoomPrice = room.RoomPrice;
                    existingRoomPost.RoomSize = room.RoomSize;
                    existingRoomPost.RoomDescription = room.RoomDescription;
                    existingRoomPost.RoomTypeId = room.RoomTypeId;
                    existingRoomPost.RoomCoordinateId = room.RoomCoordinateId;
                    existingRoomPost.Address = room.RoomAddress;
                    existingRoomPost.RoomName = $"{roomTypeName} {district}";

                    await db.SaveChangesAsync();
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

        public async Task<bool> CheckExistingCoordinateInRoomListAsync(int roomCoordinateId)
        {
            try
            {
                var room = await db.RoomPosts.SingleOrDefaultAsync(r => r.RoomCoordinateId == roomCoordinateId);
                
                if(room != null)
                {
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

        public async Task DeleteRoomCoordinateByIdAsync(int roomCoordinateId)
        {
            try
            {
                var roomCoordinate = await db.RoomCoordinates.FirstOrDefaultAsync(rc => rc.RoomCoordinateId == roomCoordinateId);
                if (roomCoordinate != null)
                {
                    db.RoomCoordinates.Remove(roomCoordinate);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public bool UpdateRoomServices(List<int?> selectedUtilities, int postId)
        {
            try
            {
                var currentRoomServices = db.RoomUtilities.Where(rs => rs.PostId == postId).ToList();

                var servicesToSkip = currentRoomServices
                    .Where(rs => selectedUtilities.Contains(rs.UtilityId)) 
                    .ToList();

                var servicesToAdd = selectedUtilities
                    .Where(serviceId => !currentRoomServices.Any(rs => rs.UtilityId == serviceId))
                    .ToList();

                var servicesToRemove = currentRoomServices
                    .Where(rs => !selectedUtilities.Contains(rs.UtilityId))
                    .ToList();

                foreach (var serviceId in servicesToAdd)
                {
                    db.RoomUtilities.Add(new RoomUtility { PostId = postId, UtilityId = serviceId });
                }

                foreach (var service in servicesToRemove)
                {
                    db.RoomUtilities.Remove(service);
                }


                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating room services: {ex.Message}");
                return false;
            }
        }

        public void UpdateRoomImages(List<string> imagesFromRoom, List<IFormFile> roomImages, int postId)
        {
            try
            {
                if (imagesFromRoom != null && roomImages != null)
                {
                    var currentRoomImages = db.RoomImages.Where(img => img.PostId == postId).ToList();

                    var imagesToRemove = currentRoomImages
                        .Where(img => !imagesFromRoom.Contains(img.ImageUrl))
                        .ToList();

                    foreach (var image in imagesToRemove)
                    {
                        db.RoomImages.Remove(image);
                    }

                    db.SaveChanges();
                }

                if (roomImages != null && roomImages.Count > 0)
                {
                    foreach (var imageFile in roomImages)
                    {
                        var fileExtension = Path.GetExtension(imageFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                        var filePath = Path.Combine("wwwroot/img/Room", uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(fileStream);
                        }

                        db.RoomImages.Add(new RoomImage
                        {
                            PostId = postId,
                            ImageUrl = uniqueFileName
                        });
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating room images: {ex.Message}");
            }
        }
        public async Task<int> GetRoomCoordinateIdByAddress(string roomAddress)
        {
            try
            {
                int? roomCoordinateId = (from r in db.RoomPosts
                                         where r.Address == roomAddress
                                         select r.RoomCoordinateId).FirstOrDefault();

                return roomCoordinateId ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<RoomPostContentVM> GetRoomPostById(int idPhong)
        {
            try
            {
                var room = (from r in db.RoomPosts
                           where r.PostId == idPhong
                           select new RoomPostContentVM
                           {
                               PostId = idPhong,
                               RoomCoordinateId = (int)r.RoomCoordinateId
                           }).FirstOrDefault();

                return room;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> HasRoomUtilities(int idPhong)
        {
            try
            {
                bool hasUtilities = await db.RoomUtilities
                        .AnyAsync(u => u.PostId == idPhong);

                return hasUtilities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task DeleteRoomUtilities(int idPhong)
        {
            try
            {
                var utilities = db.RoomUtilities.Where(u => u.PostId == idPhong);

                db.RoomUtilities.RemoveRange(utilities);

                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> HasRoomFavorites(int idPhong)
        {
            try
            {
                bool exists = await db.FavoriteListPosts.AnyAsync(f => f.PostId == idPhong);
                return exists;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task DeleteRoomFavorites(int idPhong)
        {
            try
            {
                var roomFavorites = db.FavoriteListPosts.Where(f => f.PostId == idPhong);

                db.FavoriteListPosts.RemoveRange(roomFavorites);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> HasRoomImages(int idPhong)
        {
            try
            {
                return await db.RoomImages.AnyAsync(img => img.PostId == idPhong);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task DeleteRoomImages(int idPhong)
        {
            try
            {
                var roomImages = db.RoomImages.Where(img => img.PostId == idPhong).ToList();

                if (roomImages.Any())
                {
                    db.RoomImages.RemoveRange(roomImages);
                    await db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> HasOtherPostsWithCoordinate(int roomCoordinateId)
        {
            try
            {
                var hasOtherPosts = await db.RoomPosts
                                    .Where(rp => rp.RoomCoordinateId == roomCoordinateId)
                                    .AnyAsync();
                return hasOtherPosts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task DeleteRoomPost(int idPhong)
        {
            try
            {
                var roomPostToDelete = await db.RoomPosts.FindAsync(idPhong);
                if (roomPostToDelete != null)
                {
                    db.RoomPosts.Remove(roomPostToDelete);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
