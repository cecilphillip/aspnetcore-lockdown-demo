

using System.Linq;
using DayCare.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web.Services
{
    using System.Threading.Tasks;

    public interface ISecurityService
    {
        Task<Guardian> ValidateGuardianCredentialsAsync(string email, string password);
    }

    public class SecurityService : ISecurityService
    {
        private readonly DayCareContext _dbContext;

        public SecurityService(DayCareContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guardian> ValidateGuardianCredentialsAsync(string email, string password)
        {
            var guardian = await _dbContext.Guardians.SingleOrDefaultAsync(g => g.Email == email);
            if (guardian != null)
            {
                return !string.IsNullOrEmpty(password)? guardian: null;
            }

            return null;
        }
    }
}
