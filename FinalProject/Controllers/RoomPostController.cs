using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FinalProject.Services;
using Newtonsoft.Json.Linq;
using FinalProject.Helpers;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace FinalProject.Controllers
{
    public class RoomPostController : Controller
    {
        private readonly QlptContext _db;
        private readonly RoomService _roomService;
        private readonly UserService _userService;
        private readonly RoomFeedbackService _roomFeedbackService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();

        public RoomPostController(QlptContext db, RoomService _roomService,  IHttpClientFactory _httpClientFactory, UserService userService, IConfiguration configuration, RoomFeedbackService _roomFeedbackService)
        {
            this._db = db;
            this._roomService = _roomService;
            this._httpClientFactory = _httpClientFactory;
            _userService = userService;
            this._configuration = configuration;
            client.BaseAddress = new Uri("http://localhost:7247");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            this._roomFeedbackService = _roomFeedbackService;
        }

        #region GetRoomPost
        public IActionResult Index(int? id, int page = 1, int pageSize = 6)
        {
            _roomFeedbackService.HidePostsForAllViolationsAsync();

            var roomList = _db.RoomPostVM.FromSqlRaw($"GetRoomPosts {id}").ToList();

            int totalRooms = roomList.Count;
            int totalPages = (int)Math.Ceiling((double)totalRooms / pageSize);

            var rooms = roomList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new RoomListVM
            {
                Rooms = rooms,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
        #endregion

        #region SearchRoom
        [HttpGet]
        public IActionResult SearchRoom(int? roomType, string? district, string? ward, int? adult, string? priceRange)
        {
            List<RoomPostVM> rooms = new List<RoomPostVM>();

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC SearchRoom @RoomTypeId, @District, @Ward, @Adult, @PriceRange";

                command.Parameters.Add(new SqlParameter("@RoomTypeId", (object)roomType ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@District", (object)district ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Ward", (object)ward ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Adult", (object)adult ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PriceRange", (object)priceRange ?? DBNull.Value));

                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var room = new RoomPostVM
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            RoomDescription = reader.GetString(reader.GetOrdinal("RoomDescription")),
                            RoomImage = reader.GetString(reader.GetOrdinal("RoomImage")),
                            RoomPrice = reader.GetDecimal(reader.GetOrdinal("RoomPrice")),
                            RoomSize = reader.GetDecimal(reader.GetOrdinal("RoomSize")),
                            RoomAddress = reader.GetString(reader.GetOrdinal("RoomAddress")),
                            RoomType = reader.GetString(reader.GetOrdinal("RoomType"))
                        };

                        rooms.Add(room);
                    }
                }

                _db.Database.CloseConnection();
            }

            return View("SearchRoom", rooms);
        }
        #endregion

        #region RoomDetail
        public IActionResult Detail(int id)
        {
            var roomDetail = new RoomPostDetailVM();
            var roomImages = new List<RoomImageVM>();

            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewData["GoogleMapsApiKey"] = googleMapsApiKey;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC GetRoomDetail @PostID";
                command.Parameters.Add(new SqlParameter("@PostID", id));

                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        roomDetail = new RoomPostDetailVM
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            RoomDescription = reader.GetString(reader.GetOrdinal("RoomDescription")),
                            RoomPrice = reader.GetDecimal(reader.GetOrdinal("RoomPrice")),
                            RoomSize = reader.GetDecimal(reader.GetOrdinal("RoomSize")),
                            RoomAddress = reader.GetString(reader.GetOrdinal("RoomAddress")),
                            RoomType = reader.GetString(reader.GetOrdinal("RoomType")),
                            DatePosted = reader.GetDateTime(reader.GetOrdinal("DatePosted")),
                            ExpirationDate = reader.GetDateTime(reader.GetOrdinal("ExpirationDate")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Gender = !reader.IsDBNull(reader.GetOrdinal("Gender")) && reader.GetBoolean(reader.GetOrdinal("Gender")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            UserImage = reader.GetString(reader.GetOrdinal("UserImage")),
                            UtilityNames = reader.GetString(reader.GetOrdinal("UtilityNames")),
                            UtilityDescriptions = reader.GetString(reader.GetOrdinal("UtilityDescriptions")),
                            Latitude = reader.GetDouble(reader.GetOrdinal("Latitude")),
                            Longitude = reader.GetDouble(reader.GetOrdinal("Longitude")),
                        };
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("RoomImage")))
                            {
                                roomImages.Add(new RoomImageVM
                                {
                                    RoomImage = reader.GetString(reader.GetOrdinal("RoomImage"))
                                });
                            }
                        }
                    }
                }

                _db.Database.CloseConnection();
            }

            var model = new RoomDetailAndImagesVM
            {
                RoomDetail = roomDetail,
                RoomImages = roomImages
            };

            return View(model);
        }
        #endregion

        #region UploadRoomPostView
        [Authorize]
        public IActionResult UploadRoomPost()
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var model = new UploadRoomPostVM
                {
                    Users = _userService.GetUserById(userId),
                    Utility = _roomService.GetUtilities()?.ToList() ?? new List<UtilityVM>(),
                    RoomTypes = _roomService.GetRoomType(),
                    RoomPostContentVM = new RoomPostContentVM()
                };
                return View(model);
            }

            return RedirectToAction("SignIn", "Customer");
        }
        #endregion

        #region UploadRoomPost
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadRoomPost(UploadRoomPostVM model)
        {

            ModelState.Remove("RoomTypes");
            ModelState.Remove("Users");

            if (model.RoomImages == null || model.RoomImages.Count == 0)
            {
                TempData["FailMessage"] = "Vui lòng thêm ít nhất một ảnh!";
                PopulateModelData(model);
                return View(model);
            }

            if (model.RoomPostContentVM.RoomDescription != null)
            {
                var predict = CallPythonModel(model.RoomPostContentVM.RoomDescription);

                if (!predict)
                {
                    TempData["FailMessage"] = "Mô tả phòng không đạt yêu cầu.";
                    PopulateModelData(model);
                    return View(model);
                }
            }

            if (model.RoomImages != null && model.RoomImages.Count > 0)
            {
                string invalidObjects = await ProcessRoomImagesAsync(model.RoomImages);

                if (invalidObjects != null)
                {
                    TempData["FailMessage"] = invalidObjects;
                    PopulateModelData(model);
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["FailMessage"] = "Có lỗi khi thêm bài đăng phòng!";
                PopulateModelData(model); 
                return View(model); 
            }
            else
            {
                var coordinates = await GetCoordinatesAsync(model.RoomPostContentVM.RoomAddress);

                string userInput = model.RoomPostContentVM.RoomAddress;
                string formattedAddress = coordinates.formattedAddress;


                if (coordinates.latitude == 0 && coordinates.longitude == 0 || !formattedAddress.Contains("Hồ Chí Minh") && !formattedAddress.Contains("Vietnam"))
                {
                    TempData["FailMessage"] = coordinates.latitude == 0 && coordinates.longitude == 0 ? "Địa chỉ bạn cung cấp không tồn tại!" : "Địa chỉ phải nằm trong khu vực Hồ Chí Minh!";

                    PopulateModelData(model);

                    return View(model);
                }

                bool isMatch = CompareAddresses(userInput, formattedAddress);
                if (!isMatch)
                {
                    TempData["FailMessage"] = "Địa chỉ bạn cung cấp không tồn tại!";

                    PopulateModelData(model);

                    return View(model);
                }

                int userId;

                if (!Request.Cookies.TryGetValue("user_id", out var userIdString) || !int.TryParse(userIdString, out userId))
                {
                    TempData["FailMessage"] = "Người dùng không tồn tại.";
                    return RedirectToAction("SignIn", "Customer");
                }

                if (_roomService.CheckExistingCoordinate(coordinates.latitude, coordinates.longitude))
                {
                    var roomCoordinateId = await _roomService.GetRoomCoordinateId(coordinates.latitude, coordinates.longitude);

                    if (roomCoordinateId != 0)
                    {
                        model.RoomPostContentVM.RoomCoordinateId = roomCoordinateId;

                        if (_roomService.AddNewRoomPost(model.RoomPostContentVM, userId) is int newRoomId)
                        {
                            _roomService.SaveSelectedUtilities(newRoomId, model.SelectedUtilities);
                            SaveRoomImages(newRoomId, model.RoomImages);
                            TempData["SuccessMessage"] = "Thêm mới bài viết thành công!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Thêm mới bài viết thất bại!";
                        }

                    }
                    else
                    {
                        TempData["FailMessage"] = "Tọa độ ảnh không tồn tại!";
                    }
                }
                else
                {
                    int roomCoordinateId = _roomService.AddNewRoomCoordinate(coordinates.latitude, coordinates.longitude);

                    if (roomCoordinateId != 0)
                    {
                        model.RoomPostContentVM.RoomCoordinateId = roomCoordinateId;

                        if (_roomService.AddNewRoomPost(model.RoomPostContentVM, userId) is int newRoomId)
                        {
                            _roomService.SaveSelectedUtilities(newRoomId, model.SelectedUtilities);

                            SaveRoomImages(newRoomId, model.RoomImages);

                            TempData["SuccessMessage"] = "Thêm mới bài viết thành công!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Thêm mới bài viết thất bại!";
                        }

                    }
                    else
                    {
                        TempData["FailMessage"] = "Thêm mới Tọa độ ảnh thất bại!";
                    }
                }

                PopulateModelData(model);

                return View(model);
            }
        }

        #endregion

        #region PopulateModelDate
        private void PopulateModelData(UploadRoomPostVM model)
        {
            model.RoomTypes = _roomService.GetRoomType();
            model.Utility = _roomService.GetUtilities();
            model.Users = GetUserFromCookies();
        }
        #endregion

        #region CompareAddresses
        private bool CompareAddresses(string userInput, string formattedAddress)
        {
            var userInputParts = userInput.Split(',').Select(part => part.Trim()).ToList();
            var formattedAddressParts = formattedAddress.Split(',').Select(part => part.Trim()).ToList();

            var userStreetAndDistrict = userInputParts.Take(2).ToList();
            var formattedStreetAndDistrict = formattedAddressParts.Take(2).ToList();

            return userStreetAndDistrict.SequenceEqual(formattedStreetAndDistrict);
        }

        #endregion

        #region GetCoordinate
        private async Task<(double latitude, double longitude, string formattedAddress)> GetCoordinatesAsync(string address)
        {
            var client = _httpClientFactory.CreateClient();
            string url = $"https://localhost:7127/api/geocode/getCoordinates?address={Uri.EscapeDataString(address)}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Geocode API Response: {jsonResponse}");

                try
                {
                    var result = JObject.Parse(jsonResponse);

                    if (result["latitude"] != null && result["longitude"] != null)
                    {
                        double latitude = result["latitude"].Value<double>();
                        double longitude = result["longitude"].Value<double>();

                        string formattedAddress = result["formattedAddress"]?.Value<string>() ?? "Address not available";

                        return (latitude, longitude, formattedAddress);
                    }
                    else if (result["status"] != null)
                    {
                        Console.WriteLine($"Geocoding error: {result["status"]}");
                    }
                    else
                    {
                        Console.WriteLine("Geocoding error: latitude and longitude fields are missing.");
                    }
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"JSON Parsing error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return (0, 0, null); 
        }
        #endregion

        #region GetUserFromCookies
        private UpdatePersonalInformationVM GetUserFromCookies()
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return _userService.GetUserById(userId);
            }

            return null; 
        }
        #endregion

        #region SaveRoomImages
        private void SaveRoomImages(int newRoomId, List<IFormFile> roomImages)
        {
            if (roomImages != null && roomImages.Count > 0)
            {
                foreach (var image in roomImages)
                {
                    var originalFileName = image.FileName;
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine("wwwroot/img/Room", uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    _roomService.SaveRoomImage(newRoomId, uniqueFileName); 
                }
            }
        }
        #endregion

        #region ManageRoomView

        [Authorize]
        public IActionResult ManageRoom()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var roomList = _roomService.GetRoomListByUserId(userId);

            return View(roomList);
        }
        #endregion

        #region UpdateRoomPost

        [Authorize]
        [HttpPost]
        public IActionResult UpdateRoomView(int idPhong)
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var model = new UploadRoomPostVM
                {
                    Users = _userService.GetUserById(userId),
                    Utility = _roomService.GetUtilities()?.ToList() ?? new List<UtilityVM>(),
                    RoomTypes = _roomService.GetRoomType(),
                    RoomPostContentVM = _roomService.GetRoomPostContent(idPhong),
                    SelectedUtilities = _roomService.GetUtilitiesByPostId(idPhong),
                    ImagesFromRoom = _roomService.GetImagesByPostId(idPhong)
                };
                return View(model);
            }

            return RedirectToAction("SignIn", "Customer");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateRoom(UploadRoomPostVM model)
        {
            if (model.RoomImages?.Count == 0 && model.ImagesFromRoom.Count == 0)
            {
                TempData["FailMessage"] = "Vui lòng thêm ít nhất một ảnh!";
                return RedirectToAction("ManageRoom", "RoomPost");
            }

            if (model.RoomPostContentVM.RoomDescription != null)
            {
                var predict = CallPythonModel(model.RoomPostContentVM.RoomDescription);

                if (!predict) 
                {
                    TempData["FailMessage"] = "Mô tả phòng không đạt yêu cầu.";
                    return RedirectToAction("ManageRoom", "RoomPost");
                }
            }

            if (model.RoomImages != null && model.RoomImages.Count > 0)
            {
                string invalidObjects = await ProcessRoomImagesAsync(model.RoomImages);

                if (invalidObjects != null)
                {
                    TempData["FailMessage"] = invalidObjects;
                    return RedirectToAction("ManageRoom", "RoomPost");
                }
            }

            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                if (model.RoomPostContentVM != null)
                {
                    var addressUpdateSuccess = await HandleRoomAddressUpdate(model);

                    if (addressUpdateSuccess)
                    {
                        if (model.SelectedUtilities != null)
                        {
                            _roomService.UpdateRoomServices(model.SelectedUtilities, model.RoomPostContentVM.PostId);
                            _roomService.UpdateRoomImages(model.ImagesFromRoom, model.RoomImages, model.RoomPostContentVM.PostId);
                        }

                        TempData["SuccessMessage"] = "Cập nhật phòng thành công!";
                        return RedirectToAction("ManageRoom", "RoomPost");
                    }
                    else
                    {
                        TempData["FailMessage"] = "Cập nhật địa chỉ phòng thất bại! Vui lòng kiểm tra lại.";
                        return RedirectToAction("ManageRoom", "RoomPost");
                    }
                }
                else
                {
                    TempData["FailMessage"] = "Thông tin phòng không hợp lệ!";
                    return RedirectToAction("ManageRoom", "RoomPost");
                }
            }

            TempData["FailMessage"] = "Vui lòng đăng nhập để tiếp tục!";
            return RedirectToAction("SignIn", "Customer");
        }



        private async Task<bool> HandleRoomAddressUpdate(UploadRoomPostVM model)
        {
            int oldRoomCoordinateId = model.RoomPostContentVM.RoomCoordinateId;

            if (!string.IsNullOrEmpty(model.RoomPostContentVM.RoomAddress))
            {
                int newRoomCoordinateId = 0;

                if (await _roomService.CheckRoomAddresAsync(model.RoomPostContentVM.RoomAddress))
                {
                    newRoomCoordinateId = await _roomService.GetRoomCoordinateIdByAddress(model.RoomPostContentVM.RoomAddress);
                }
                else
                {
                    var coordinates = await GetCoordinatesAsync(model.RoomPostContentVM.RoomAddress);

                    string userInput = model.RoomPostContentVM.RoomAddress;
                    string formattedAddress = coordinates.formattedAddress;


                    if (coordinates.latitude == 0 && coordinates.longitude == 0 || !formattedAddress.Contains("Hồ Chí Minh") && !formattedAddress.Contains("Vietnam"))
                    {
                        TempData["FailMessage"] = coordinates.latitude == 0 && coordinates.longitude == 0 ? "Địa chỉ bạn cung cấp không tồn tại!" : "Địa chỉ phải nằm trong khu vực Hồ Chí Minh!";
                        return false;
                    }

                    bool isMatch = CompareAddresses(userInput, formattedAddress);
                    if (!isMatch)
                    {
                        TempData["FailMessage"] = "Địa chỉ bạn cung cấp không tồn tại!";
                        return false;
                    }

                    newRoomCoordinateId = await _roomService.GetRoomCoordinateId(coordinates.latitude, coordinates.longitude);

                    if (newRoomCoordinateId == 0)
                    {
                        newRoomCoordinateId = _roomService.AddNewRoomCoordinate(coordinates.latitude, coordinates.longitude);

                        if (newRoomCoordinateId == 0)
                        {
                            return false; 
                        }
                    }
                }
                model.RoomPostContentVM.RoomCoordinateId = newRoomCoordinateId;

                if (await _roomService.UpdateRoomPostAsync(model.RoomPostContentVM))
                {
                    if (oldRoomCoordinateId != newRoomCoordinateId)
                    {
                        bool isCoordinateInUse = await _roomService.CheckExistingCoordinateInRoomListAsync(oldRoomCoordinateId);
                        if (!isCoordinateInUse)
                        {
                            await _roomService.DeleteRoomCoordinateByIdAsync(oldRoomCoordinateId);
                        }
                    }
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region DetectImageWithYOLO
        private async Task<string> ProcessRoomImagesAsync(List<IFormFile> roomImages)
        {
            foreach (var image in roomImages)
            {
                var rootPath = @"D:\HKI 2024-2025\SmartRoomManagement\FinalProject";

                var imagePath = Path.Combine(rootPath, "wwwroot", "img", "Temp", $"{DateTime.Now.Ticks}_{image.FileName}");

                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    string invalidObjects = await DetectImageWithYOLO(imagePath);

                    if (invalidObjects != null)
                    {
                        var objects = invalidObjects.Split(',');  
                        var translatedObjects = objects.Select(obj => TranslateObject.Translate(obj.Trim())).ToList();

                        return $"Ảnh chứa các đối tượng không phù hợp: {string.Join(", ", translatedObjects)}";
                    }
                }
                finally
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            return null;
        }

        private async Task<string> CallPythonScript(string imagePath)
        {
            var pythonPath = @"D:\HKI 2024-2025\SmartRoomManagement\FinalProject\Scripts\yolov8_env\Scripts\python.exe";
            var scriptPath = @"D:\HKI 2024-2025\SmartRoomManagement\FinalProject\Scripts\yolo_detection.py";

            using (var process = new Process())
            {
                process.StartInfo.FileName = pythonPath;
                process.StartInfo.Arguments = $"\"{scriptPath}\" \"{imagePath}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                return output;
            }
        }


        private async Task<string> DetectImageWithYOLO(string imagePath)
        {
            var result = await Task.Run(() =>
            {
                try
                {
                    string output = CallPythonScript(imagePath).Result;  

                    Console.WriteLine($"Python output: {output}");

                    if (output.Contains("unsuitable objects"))
                    {
                        var unsuitableObjects = output.Split(new[] { "unsuitable objects:" }, StringSplitOptions.None)[1].Trim();
                        return unsuitableObjects; 
                    }

                    return null; 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in detection: {ex.Message}");
                    return "Error in detection"; 
                }
            });

            return result;
        }

        #endregion

        #region TextAnalysis
        private bool CallPythonModel(string text)
        {
            try
            {
                var pythonPath = @"D:\HKI 2024-2025\SmartRoomManagement\FinalProject\Scripts\yolov8_env\Scripts\python.exe"; 
                string pythonScriptPath = @"D:\HKI 2024-2025\SmartRoomManagement\FinalProject\Scripts\app.py";  

                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{pythonScriptPath}\" \"{text}\"",  
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,  
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        TempData["FailMessage"] = "Không thể khởi động tiến trình Python.";
                        return false;
                    }

                    using (var reader = process.StandardOutput)
                    using (var errorReader = process.StandardError)
                    {
                        string result = reader.ReadToEnd();
                        string error = errorReader.ReadToEnd();  

                        if (!string.IsNullOrEmpty(error))
                        {
                            TempData["FailMessage"] = "Lỗi từ Python: " + error;
                            return false;
                        }

                        if (string.IsNullOrEmpty(result))
                        {
                            TempData["FailMessage"] = "Không có kết quả trả về từ Python.";
                            return false;
                        }

                        if (result.Contains("Good"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["FailMessage"] = "Đã xảy ra lỗi khi gọi Python: " + ex.Message;
                return false;
            }
        }

        #endregion

        #region DeleteRoomPost
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteRoomPost(int idPhong)
        {
            var roomPost = await _roomService.GetRoomPostById(idPhong);
            if (roomPost == null)
            {
                TempData["FailMessage"] = "Phòng không tồn tại!";
                return RedirectToAction("ManageRoom", "RoomPost");
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Xóa tất cả bản ghi liên quan trong RoomUtilities
                    if (await _roomService.HasRoomUtilities(idPhong))
                    {
                        await _roomService.DeleteRoomUtilities(idPhong);
                    }

                    // Xóa tất cả bản ghi liên quan trong RoomFavoritePosts
                    if (await _roomService.HasRoomFavorites(idPhong))
                    {
                        await _roomService.DeleteRoomFavorites(idPhong);
                    }

                    // Xóa tất cả bản ghi liên quan trong RoomImages
                    if (await _roomService.HasRoomImages(idPhong))
                    {
                        await _roomService.DeleteRoomImages(idPhong);
                    }

                    // Xóa tất cả phản hồi liên quan trong RoomFeedbacks (nếu có)
                    //if (await _roomService.HasRoomFeedbacks(idPhong))
                    //{
                    //    await _roomService.DeleteRoomFeedbacks(idPhong);
                    //}

                    // Cuối cùng, xóa bài đăng phòng từ RoomPosts
                    await _roomService.DeleteRoomPost(idPhong);

                    // Lấy `roomCoordinateId` và kiểm tra xem nó còn được dùng ở bài đăng khác không
                    int roomCoordinateId = roomPost.RoomCoordinateId;
                    if (!await _roomService.HasOtherPostsWithCoordinate(roomCoordinateId))
                    {
                        await _roomService.DeleteRoomCoordinateByIdAsync(roomCoordinateId);
                    }

                    transaction.Commit();

                    TempData["SuccessMessage"] = "Xóa phòng thành công!";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["FailMessage"] = "Đã có lỗi xảy ra khi xóa phòng. Vui lòng thử lại!";
                }
            }

            return RedirectToAction("ManageRoom", "RoomPost");
        }
        #endregion
    }
}
