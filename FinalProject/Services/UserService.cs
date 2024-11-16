using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services
{
    public class UserService
    {
        private readonly QlptContext db;

        public UserService(QlptContext db)
        {
            this.db = db;
        }

        public async Task<bool> CreateUserAsync(User user, RegisterVM register)
        {
            user.RandomKey = MyUtil.GenerateRandomKey();
            user.Password = register.password.ToMd5Hash(user.RandomKey);
            user.IsValid = true;
            user.UserTypeId = register.accountType;

            try
            {
                await db.AddAsync(user);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> ChangePassword(User user, UpdatePasswordVM model)
        {
            try
            {
                var _user = await db.Users.SingleOrDefaultAsync(u => u.UserId == user.UserId);

                if (_user == null)
                {
                    return false; 
                }
                else
                {
                    _user.Password = model.newPassword.ToMd5Hash(_user.RandomKey);

                    await db.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }


        public async Task<User> CheckUserAsync(LoginVM model)
        {
            var customer = db.Users.SingleOrDefault(u => u.Username == model.username);
            return customer;
        }

        public async Task<User> getUserById(int? userId)
        {
            if (userId == null)
            {
                return null;
            }

            var user = db.Users.SingleOrDefault(u => u.UserId == userId);

            return user;
        }

        public async Task<bool> UpdateUserInformationAsync(User user, IFormFile userimage)
        {
            try
            {
                var existingUser = await db.Users.FindAsync(user.UserId);

                if (existingUser == null)
                {
                    return false;
                }

                existingUser.Fullname = user.Fullname;
                existingUser.Dob = user.Dob;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Gender = user.Gender;
                if(userimage != null)
                {
                    existingUser.UserImage = user.UserImage;
                }    

                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<string> UpdateUserImageAsync(User existingUser, IFormFile imageFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(existingUser.UserImage))
                {
                    var oldImagePath = Path.Combine("wwwroot", existingUser.UserImage.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                var filePath = Path.Combine("wwwroot/img/User", fileName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }    

                string img = "/img/User/" + fileName;

                return img;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task<bool> VerifyCurrentPassword(string inputPassword, string storedPassword, string saltKey)
        {
            string hashedInputPassword = inputPassword.ToMd5Hash(saltKey);
            return hashedInputPassword == storedPassword;
        }

        public async Task<bool> EmailExistAsync(string email)
        {
            try
            {
                var result = db.Users.Where(u => u.Email == email).FirstOrDefault();

                if (result == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var token = Guid.NewGuid().ToString();

            var existingUser = db.Users.Where(u => u.Email == email).FirstOrDefault();

            if (existingUser == null)
            {
                return null;
            }

            existingUser.ResetToken = token;
            existingUser.TokenCreateAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return token;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string password)
        {
            var user = await db.Users
                .Where(u => u.Email == email && u.ResetToken == token && u.TokenCreateAt > DateTime.UtcNow.AddHours(-24))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Token không hợp lệ hoặc đã hết hạn." });
            }

            user.Password = password.ToMd5Hash(user.RandomKey); 
            user.ResetToken = null; 
            user.TokenCreateAt = null; 

            await db.SaveChangesAsync();

            return IdentityResult.Success; 
        }

        public UpdatePersonalInformationVM GetUserById(int userId)
        {
            try
            {
                var user = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
                if (user == null)
                {
                    return null;
                }

                var registerVM = new UpdatePersonalInformationVM
                {
                    fullname = user.Fullname,
                    email = user.Email
                };

                return registerVM;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
