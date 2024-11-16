using FinalProject.Data;
using FinalProject.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services.Admin
{
    public class AccessManagementService
    {
        private readonly QlptContext db;

        public AccessManagementService(QlptContext db)
        {
            this.db = db;
        }

        public List<UserTypeVM> GetAllUserType()
        {
            try
            {
                var userTypeList = (from ut in db.UserTypes
                                    select new UserTypeVM
                                    {
                                        UserTypeId = ut.UserTypeId,
                                        UserTypeName = ut.TypeName
                                    }).ToList();
                return userTypeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public UserTypeVM GetUserTypeById(int userTypeId)
        {
            try
            {
                var userType = db.UserTypes.FirstOrDefault(ut => ut.UserTypeId == userTypeId);

                if (userType != null)
                {
                    UserTypeVM userTypeVM = new UserTypeVM
                    {
                        UserTypeId = userType.UserTypeId,
                        UserTypeName = userType.TypeName
                    };

                    return userTypeVM;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public bool UpdateUserType(UserTypeVM userTypeVM)
        {
            try
            {
                var existingUserType = db.UserTypes.FirstOrDefault(ut => ut.UserTypeId == userTypeVM.UserTypeId);

                if (existingUserType != null)
                {
                    existingUserType.TypeName = userTypeVM.UserTypeName;

                    db.SaveChanges();

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

        public bool CanDeleteUserType(int userTypeId)
        {
            return !db.Users.Any(u => u.UserTypeId == userTypeId);
        }

        public bool DeleteUserType(int userTypeId)
        {
            var userType = db.UserTypes.FirstOrDefault(ut => ut.UserTypeId == userTypeId);
            if (userType == null)
            {
                return false; 
            }

            db.UserTypes.Remove(userType);
            db.SaveChanges();
            return true;
        }

        public bool AddUserType(UserTypeVM userTypeVM)
        {
            try
            {
                var userType = new UserType
                {
                    TypeName = userTypeVM.UserTypeName
                };

                db.UserTypes.Add(userType);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<UserTypeVM> SearchUserTypes(string searchQuery)
        {
            try
            {
                return db.UserTypes
                    .Where(u => u.TypeName.Contains(searchQuery))
                    .Select(u => new UserTypeVM
                    {
                        UserTypeId = u.UserTypeId,
                        UserTypeName = u.TypeName
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
