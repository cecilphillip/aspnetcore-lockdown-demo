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
