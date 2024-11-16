using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.ViewModels.Admin;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FinalProject.Services.Admin
{
    public class UserManagementService
    {
        private readonly QlptContext db;

        public UserManagementService(QlptContext db)
        {
            this.db = db;
        }

        public async Task<UserManagementVM?> GetUserByIdAsync(int id)
        {
            return await db.Users
                .Include(u => u.UserType)
                .Where(u => u.UserId == id)
                .Select(u => new UserManagementVM
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    UserTypeId = u.UserTypeId,
                    IsValid = u.IsValid,
                    Dob = u.Dob,
                    TypeName = u.UserType != null ? u.UserType.TypeName : null
                }).FirstOrDefaultAsync();
        }

      
        public async Task<bool> CreateUserAsync(UserManagementVM userVM)
        {
            string randomKey = MyUtil.GenerateRandomKey();

            var user = new User
            {

                Username = userVM.Username,
                Fullname = userVM.Fullname,
                Email = userVM.Email,
                Phone = userVM.Phone,
                UserTypeId = userVM.UserTypeId,
                IsValid = userVM.IsValid,
                RandomKey = randomKey,
                Password = userVM.Password.ToMd5Hash(randomKey),
          
                Dob = userVM.Dob
            };
            db.Users.Add(user);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(UserManagementVM userVM)
        {
            var user = await db.Users.FindAsync(userVM.UserId);
            if (user == null) return false;


            user.Fullname = userVM.Fullname;
            user.Email = userVM.Email;
            user.Phone = userVM.Phone;
            user.UserTypeId = userVM.UserTypeId;
            user.IsValid = userVM.IsValid;
            user.Dob = userVM.Dob;

            return await db.SaveChangesAsync() > 0;
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await db.Users.FindAsync(id);
            if(user == null) return false;

            db.Users.Remove(user);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<PagedListVM<UserManagementVM>> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            var totalItems = await db.Users.CountAsync();
            var users = await db.Users
                .OrderBy(u => u.Username)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.UserType)
                .Select(u => new UserManagementVM
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    UserTypeId = u.UserTypeId,
                    IsValid = u.IsValid,
                    Dob = u.Dob,
                    TypeName = u.UserType != null ? u.UserType.TypeName : null
                })
                .ToListAsync();

            return new PagedListVM<UserManagementVM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = users
            };
        }



        public async Task<PagedListVM<UserManagementVM>> SearchUsersAsync(string searchQuery, int pageNumber, int pageSize)
        {
            var query = db.Users
                .Include(u => u.UserType)
                .Where(u => u.Username.Contains(searchQuery) || u.Fullname.Contains(searchQuery) || u.Email.Contains(searchQuery))
                .Select(u => new UserManagementVM
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    UserTypeId = u.UserTypeId,
                    IsValid = u.IsValid,
                    Dob = u.Dob,
                    TypeName = u.UserType != null ? u.UserType.TypeName : null
                });

            int totalItems = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Username)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedListVM<UserManagementVM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = users
            };
        }

        public async Task<PagedListVM<UserManagementVM>> GetPagedUsersByUserTypeAsync(int userTypeId, int pageNumber, int pageSize)
        {
            var query = db.Users
                .Include(u => u.UserType)
                .Where(u => u.UserTypeId == userTypeId)
                .Select(u => new UserManagementVM
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    UserTypeId = u.UserTypeId,
                    IsValid = u.IsValid,
                    Dob = u.Dob,
                    TypeName = u.UserType != null ? u.UserType.TypeName : null
                });

            int totalItems = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Username)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedListVM<UserManagementVM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = users
            };
        }

    }
}
