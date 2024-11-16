using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers.API
{
    [Route("api/districts")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetDistricts()
        {
            var districts = new List<object>
            {
                new { id = 1, name = "Quận 1" },
                new { id = 2, name = "Quận 2" },
                new { id = 3, name = "Quận 3" },
                new { id = 4, name = "Quận 4" },
                new { id = 5, name = "Quận 5" },
                new { id = 6, name = "Quận 6" },
                new { id = 7, name = "Quận 7" },
                new { id = 8, name = "Quận 8" },
                new { id = 9, name = "Quận 9" },
                new { id = 10, name = "Quận 10" },
                new { id = 11, name = "Quận 11"},
                new { id = 12, name = "Quận 12"},
                new { id = 13, name = "Quận Bình Thạnh"},
                new { id = 14, name = "Quận Gò Vấp"},
                new { id = 15, name = "Quận Phú Nhuận"},
                new { id = 16, name = "Quận Tân Bình"},
                new { id = 17, name = "Quận Bình Tân"}
            };

            return Ok(districts);
        }

        [HttpGet("{districtId}/wards")]
        public IActionResult GetWards(int districtId)
        {
            var wards = new Dictionary<int, List<object>>
            {
                { 1, new List<object>
                    {
                        new { id = 1, name = "Phường Bến Nghé" },
                        new { id = 2, name = "Phường Bến Thành" },
                        new { id = 3, name = "Phường Cầu Kho" },
                        new { id = 4, name = "Phường Cầu Ông Lãnh" },
                        new { id = 5, name = "Phường Cô Giang" },
                        new { id = 6, name = "Phường Nguyễn Cư Trinh" },
                        new { id = 7, name = "Phường Nguyễn Thái Bình" },
                        new { id = 8, name = "Phường Phạm Ngũ Lão" },
                        new { id = 9, name = "Phường Tân Định" },
                        new { id = 10, name = "Phường Đa Kao" }
                    }
                },
                { 2, new List<object>
                    {
                        new { id = 11, name = "Phường An Khánh" },
                        new { id = 12, name = "Phường An Lợi Đông" },
                        new { id = 13, name = "Phường An Phú" },
                        new { id = 14, name = "Phường Bình An" },
                        new { id = 15, name = "Phường Bình Khánh" },
                        new { id = 16, name = "Phường Bình Trưng Tây" },
                        new { id = 17, name = "Phường Bình Trưng Đông" },
                        new { id = 18, name = "Phường Cát Lái" },
                        new { id = 19, name = "Phường Thạnh Mỹ Lợi" },
                        new { id = 20, name = "Phường Thảo Điền" }
                    }
                },
                { 3, new List<object>
                    {
                        new { id = 21, name = "Phường 1" },
                        new { id = 22, name = "Phường 2" },
                        new { id = 23, name = "Phường 3" },
                        new { id = 24, name = "Phường 4" },
                        new { id = 25, name = "Phường 5" },
                        new { id = 26, name = "Phường 6" },
                        new { id = 27, name = "Phường 7" },
                        new { id = 28, name = "Phường 8" },
                        new { id = 29, name = "Phường 9" },
                        new { id = 30, name = "Phường 10" },
                        new { id = 31, name = "Phường 11" },
                        new { id = 32, name = "Phường 12" },
                        new { id = 33, name = "Phường 13" },
                        new { id = 34, name = "Phường 14" }

                    }
                },
                { 4, new List<object>
                    {
                        new { id = 35, name = "Phường 1" },
                        new { id = 36, name = "Phường 2" },
                        new { id = 37, name = "Phường 3" },
                        new { id = 38, name = "Phường 4" },
                        new { id = 39, name = "Phường 5" },
                        new { id = 40, name = "Phường 6" },
                        new { id = 41, name = "Phường 7" },
                        new { id = 42, name = "Phường 8" },
                        new { id = 43, name = "Phường 9" },
                        new { id = 44, name = "Phường 10" },
                        new { id = 45, name = "Phường 11" },
                        new { id = 46, name = "Phường 12" },
                        new { id = 47, name = "Phường 13" },
                        new { id = 48, name = "Phường 14" },
                        new { id = 49, name = "Phường 15" },
                        new { id = 50, name = "Phường 16" },
                        new { id = 51, name = "Phường 17" },
                        new { id = 52, name = "Phường 18" }



                    }
                },
                { 5, new List<object>
                    {
                        new { id = 53, name = "Phường 1" },
                        new { id = 54, name = "Phường 2" },
                        new { id = 55, name = "Phường 3" },
                        new { id = 56, name = "Phường 4" },
                        new { id = 57, name = "Phường 5" },
                        new { id = 58, name = "Phường 6" },
                        new { id = 59, name = "Phường 7" },
                        new { id = 60, name = "Phường 8" },
                        new { id = 61, name = "Phường 9" },
                        new { id = 62, name = "Phường 10" },
                        new { id = 63, name = "Phường 11" },
                        new { id = 64, name = "Phường 12" },
                        new { id = 65, name = "Phường 13" },
                        new { id = 66, name = "Phường 14" },
                        new { id = 67, name = "Phường 15" }

                    }
                },
                { 6, new List<object>
                    {
                        new { id = 68, name = "Phường 1" },
                        new { id = 69, name = "Phường 2" },
                        new { id = 70, name = "Phường 3" },
                        new { id = 71, name = "Phường 4" },
                        new { id = 72, name = "Phường 5" },
                        new { id = 73, name = "Phường 6" },
                        new { id = 74, name = "Phường 7" },
                        new { id = 75, name = "Phường 8" },
                        new { id = 76, name = "Phường 9" },
                        new { id = 77, name = "Phường 10" },
                        new { id = 78, name = "Phường 11" },
                        new { id = 79, name = "Phường 12" },
                        new { id = 80, name = "Phường 13" },
                        new { id = 81, name = "Phường 14" },
                        new { id = 82, name = "Phường 15" },
                        new { id = 83, name = "Phường 16" },
                        new { id = 84, name = "Phường 17" }

                    }
                },
                { 7, new List<object>
                    {
                        new { id = 85, name = "Phường Bình Thuận" },
                        new { id = 86, name = "Phường Phú Mỹ" },
                        new { id = 87, name = "Phường Phú Thuận" },
                        new { id = 88, name = "Phường Tân Hưng" },
                        new { id = 89, name = "Phường Tân Kiểng" },
                        new { id = 90, name = "Phường Tân Phong" },
                        new { id = 91, name = "Phường Tân Phú" },
                        new { id = 92, name = "Phường Tân Quy" },
                        new { id = 93, name = "Phường Tân Thuận Tây" },
                        new { id = 94, name = "Phường Tân Thuận Đông" }


                    }
                },
                { 8, new List<object>
                    {
                        new { id = 95, name = "Phường 1" },
                        new { id = 96, name = "Phường 2" },
                        new { id = 97, name = "Phường 3" },
                        new { id = 98, name = "Phường 4" },
                        new { id = 99, name = "Phường 5" },
                        new { id = 100, name = "Phường 6" },
                        new { id = 101, name = "Phường 7" },
                        new { id = 102, name = "Phường 8" },
                        new { id = 103, name = "Phường 9" },
                        new { id = 104, name = "Phường 10" },
                        new { id = 105, name = "Phường 11" },
                        new { id = 106, name = "Phường 12" },
                        new { id = 107, name = "Phường 13" },
                        new { id = 108, name = "Phường 14" },
                        new { id = 109, name = "Phường 15" }

                    }
                },
                { 9, new List<object>
                    {
                        new { id = 110, name = "Phường Hiệp Phú" },
                        new { id = 111, name = "Phường Long Bình" },
                        new { id = 112, name = "Phường Long Phước" },
                        new { id = 113, name = "Phường Long Thạnh Mỹ" },
                        new { id = 114, name = "Phường Long Trường" },
                        new { id = 115, name = "Phường Phú Hữu" },
                        new { id = 116, name = "Phường Phước Bình" },
                        new { id = 117, name = "Phường Phước Long A" },
                        new { id = 118, name = "Phường Phước Long B" },
                        new { id = 119, name = "Phường Tân Phú" },
                        new { id = 120, name = "Phường Tăng Nhơn Phú A" },
                        new { id = 121, name = "Phường Tăng Nhơn Phú B" },
                        new { id = 122, name = "Phường Trường Thạnh" }

                    }
                },
                { 10, new List<object>
                    {
                        new { id = 123, name = "Phường 1" },
                        new { id = 124, name = "Phường 2" },
                        new { id = 125, name = "Phường 3" },
                        new { id = 126, name = "Phường 4" },
                        new { id = 127, name = "Phường 5" },
                        new { id = 128, name = "Phường 6" },
                        new { id = 129, name = "Phường 7" },
                        new { id = 130, name = "Phường 8" },
                        new { id = 131, name = "Phường 9" },
                        new { id = 132, name = "Phường 10" },
                        new { id = 133, name = "Phường 11" },
                        new { id = 134, name = "Phường 12" },
                        new { id = 135, name = "Phường 13" },
                        new { id = 136, name = "Phường 14" },
                        new { id = 137, name = "Phường 15" }
                    }
                },
                { 11, new List<object>
                    {
                        new { id = 138, name = "Phường 1" },
                        new { id = 139, name = "Phường 2" },
                        new { id = 140, name = "Phường 3" },
                        new { id = 141, name = "Phường 4" },
                        new { id = 142, name = "Phường 5" },
                        new { id = 143, name = "Phường 6" },
                        new { id = 144, name = "Phường 7" },
                        new { id = 145, name = "Phường 8" },
                        new { id = 146, name = "Phường 9" },
                        new { id = 147, name = "Phường 10" },
                        new { id = 148, name = "Phường 11" },
                        new { id = 149, name = "Phường 12" },
                        new { id = 150, name = "Phường 13" },
                        new { id = 151, name = "Phường 14" },
                        new { id = 152, name = "Phường 15" },
                        new { id = 153, name = "Phường 16" }

                    }
                },
                { 12, new List<object>
                    {
                        new { id = 154, name = "Phường An Phú Đông" },
                        new { id = 155, name = "Phường Hiệp Thành" },
                        new { id = 156, name = "Phường Tân Chánh Hiệp" },
                        new { id = 157, name = "Phường Tân Hưng Thuận" },
                        new { id = 158, name = "Phường Tân Thới Hiệp" },
                        new { id = 159, name = "Phường Tân Thới Nhất" },
                        new { id = 160, name = "Phường Thạnh Lộc" },
                        new { id = 161, name = "Phường Thạnh Xuân" },
                        new { id = 162, name = "Phường Thới An" },
                        new { id = 163, name = "Phường Trung Mỹ Tây" },
                        new { id = 164, name = "Phường Đông Hưng Thuận" }

                    }
                },
                { 13, new List<object>
                    {
                        new { id = 165, name = "Phường 1" },
                        new { id = 166, name = "Phường 2" },
                        new { id = 167, name = "Phường 3" },
                        new { id = 168, name = "Phường 5" },
                        new { id = 169, name = "Phường 6" },
                        new { id = 170, name = "Phường 7" },
                        new { id = 171, name = "Phường 11" },
                        new { id = 172, name = "Phường 12" },
                        new { id = 173, name = "Phường 13" },
                        new { id = 174, name = "Phường 14" },
                        new { id = 175, name = "Phường 15" },
                        new { id = 176, name = "Phường 17" },
                        new { id = 177, name = "Phường 19" },
                        new { id = 178, name = "Phường 21" },
                        new { id = 179, name = "Phường 22" },
                        new { id = 180, name = "Phường 24" },
                        new { id = 181, name = "Phường 25" },
                        new { id = 182, name = "Phường 26" },
                        new { id = 183, name = "Phường 27" },
                        new { id = 184, name = "Phường 28" }
                    }
                },
                { 14, new List<object>
                    {
                        new { id = 185, name = "Phường 1" },
                        new { id = 186, name = "Phường 2" },
                        new { id = 187, name = "Phường 3" },
                        new { id = 188, name = "Phường 4" },
                        new { id = 189, name = "Phường 5" },
                        new { id = 190, name = "Phường 6" },
                        new { id = 191, name = "Phường 7" },
                        new { id = 192, name = "Phường 8" },
                        new { id = 193, name = "Phường 9" },
                        new { id = 194, name = "Phường 10" },
                        new { id = 195, name = "Phường 11" },
                        new { id = 196, name = "Phường 12" },
                        new { id = 197, name = "Phường 13" },
                        new { id = 198, name = "Phường 14" },
                        new { id = 199, name = "Phường 15" },
                        new { id = 200, name = "Phường 16" }
                    }
                },
                { 15, new List<object>
                    {
                        new { id = 201, name = "Phường 1" },
                        new { id = 202, name = "Phường 3" },
                        new { id = 203, name = "Phường 4" },
                        new { id = 204, name = "Phường 5" },
                        new { id = 205, name = "Phường 6" },
                        new { id = 206, name = "Phường 7" },
                        new { id = 207, name = "Phường 8" },
                        new { id = 208, name = "Phường 9" },
                        new { id = 209, name = "Phường 10" },
                        new { id = 210, name = "Phường 11" },
                        new { id = 211, name = "Phường 12" },
                        new { id = 212, name = "Phường 13" },
                        new { id = 213, name = "Phường 14" },
                        new { id = 214, name = "Phường 15" },
                        new { id = 215, name = "Phường 16" }
                    }
                },
                { 16, new List<object>
                    {
                        new { id = 216, name = "Phường 1" },
                        new { id = 217, name = "Phường 2" },
                        new { id = 218, name = "Phường 3" },
                        new { id = 219, name = "Phường 4" },
                        new { id = 220, name = "Phường 5" },
                        new { id = 221, name = "Phường 6" },
                        new { id = 222, name = "Phường 7" },
                        new { id = 223, name = "Phường 8" },
                        new { id = 224, name = "Phường 9" },
                        new { id = 225, name = "Phường 10" },
                        new { id = 226, name = "Phường 11" },
                        new { id = 227, name = "Phường 12" }
                    }
                },
                { 17, new List<object>
                    {
                        new { id = 228, name = "Phường An Lạc" },
                        new { id = 229, name = "Phường An Lạc A" },
                        new { id = 230, name = "Phường Bình Hưng Hòa" },
                        new { id = 231, name = "Phường Bình Hưng Hòa A" },
                        new { id = 232, name = "Phường Bình Hưng Hòa B" },
                        new { id = 233, name = "Phường Bình Trị Đông" },
                        new { id = 234, name = "Phường Bình Trị Đông A" },
                        new { id = 235, name = "Phường Bình Trị Đông B" },
                        new { id = 236, name = "Phường Tân Tạo" },
                        new { id = 237, name = "Phường Tân Tạo A" }
                    }
                }
            };

            if (wards.ContainsKey(districtId))
            {
                return Ok(wards[districtId]);
            }

            return NotFound();
        }

    }
}
