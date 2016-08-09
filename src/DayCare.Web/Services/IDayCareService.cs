using System.Collections.Generic;
using System.Linq;
using DayCare.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web.Services
{
    using System.Threading.Tasks;

    public interface IDayCareService
    {
        Task<Guardian> ValidateGuardianCredentialsAsync(string email, string password);
        Task<Guardian> GetGuardianAsync(int id);
        Task<IEnumerable<ChildActivity>> GetActivitiesForChildAsync(int id);
        Task<Staff> ValidateStaffCredentialsAsync(string email, string password);
        Task<Staff> GetStaffMemberAsync(int id);
        Task<Staff> GetStaffMemberAsync(string name);
        Task<IEnumerable<Child>> GetChildrenAsync();
        Task<Child> GetChildAsync(int id);
        Task AddNoteForChildAsync(ChildActivity childActivity);
        Task<bool> ChildExistsAsync(int childId);
    }

    public class DayCareService : IDayCareService
    {
        private readonly DayCareContext _dbContext;

        public DayCareService(DayCareContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guardian> ValidateGuardianCredentialsAsync(string email, string password)
        {
            var guardian = await _dbContext.Guardians.SingleOrDefaultAsync(g => g.Email == email);
            if (guardian != null)
            {
                return !string.IsNullOrEmpty(password) ? guardian : null;
            }

            return null;
        }

        public async Task<Staff> ValidateStaffCredentialsAsync(string email, string password)
        {
            var staff = await _dbContext.Staff.SingleOrDefaultAsync(g => g.Email == email);
            if (staff != null)
            {
                return !string.IsNullOrEmpty(password) ? staff : null;
            }

            return null;
        }

        public async Task<Staff> GetStaffMemberAsync(int id)
        {
            var staffMember = await _dbContext.Staff.SingleOrDefaultAsync(s => s.Id == id);
            return staffMember;
        }

        public async Task<Staff> GetStaffMemberAsync(string name)
        {
            var staffMember = await _dbContext.Staff.SingleOrDefaultAsync(s => s.FirstName == name);
            return staffMember;
        }

        public async Task<IEnumerable<Child>> GetChildrenAsync()
        {
            return await _dbContext.Children.ToListAsync();
        }

        public async Task<Child> GetChildAsync(int id)
        {
            return await _dbContext.Children.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddNoteForChildAsync(ChildActivity childActivity)
        {
            _dbContext.ChildrenActivities.Add(childActivity);
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> ChildExistsAsync(int childId)
        {
            return _dbContext.Children.AnyAsync(c => c.Id == childId);
        }

        public async Task<Guardian> GetGuardianAsync(int id)
        {
            var guardian = await _dbContext.Guardians.SingleOrDefaultAsync(g => g.Id == id);
            return guardian;
        }

        public async Task<IEnumerable<ChildActivity>> GetActivitiesForChildAsync(int id)
        {
            var activities = await _dbContext.ChildrenActivities.Where(ca => ca.ChildId == id).ToListAsync();
            return activities;
        }
    }
}
