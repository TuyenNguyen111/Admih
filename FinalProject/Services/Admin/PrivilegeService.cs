using FinalProject.Data;
using FinalProject.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services.Admin
{
    public class PrivilegeService
    {
        private readonly QlptContext db;

        public PrivilegeService(QlptContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Privilege>> GetAllPrivilegesAsync()
        {
            return await db.Privileges
                .Include(p => p.UserType)
                .Include(p => p.PageAddress)
                .ToListAsync();
        }

        private async Task<IEnumerable<int>> GetPageAddressIdByUserTypeAsync(int? userTypeId)
        {
            try
            {
                if (userTypeId == null)
                {
                    return Enumerable.Empty<int>(); 
                }

                var pageAddressId = await db.Privileges
                        .Where(p => p.UserTypeId == userTypeId && p.IsPrivileged == true)
                        .Select(p => p.PageAddressId)
                        .Where(id => id.HasValue) 
                        .Select(id => id.Value)  
                        .ToListAsync();

                return pageAddressId;
            }
            catch (Exception ex)
            {

                return Enumerable.Empty<int>();
            }
        }

        public async Task<IEnumerable<PageAddressVM>> GetPageAddressesForUser(int? userTypeId)
        {
            try
            {
                var pageAddressIds = await GetPageAddressIdByUserTypeAsync(userTypeId);

                if (pageAddressIds == null || !pageAddressIds.Any())
                {
                    return Enumerable.Empty<PageAddressVM>(); 
                }

                var pageAddresses = await db.PageAddresses
                                             .Where(pa => pageAddressIds.Contains(pa.PageAddressId))
                                             .ToListAsync();

                var pageAddressVMs = pageAddresses.Select(pa => new PageAddressVM
                {
                    PageAddressId = pa.PageAddressId,
                    PageName = pa.PageName,
                    Url = pa.Url,
                }).ToList();

                return pageAddressVMs; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<PageAddressVM>(); 
            }
        }

    }
}
