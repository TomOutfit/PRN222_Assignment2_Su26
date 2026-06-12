using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Data;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
    {
        public SystemAccountRepository(FUNewsManagementDbContext context) : base(context)
        {
        }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public async Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role)
        {
            return await _context.SystemAccounts
                .Where(a => a.AccountRole == role)
                .OrderBy(a => a.AccountName)
                .ToListAsync();
        }
    }
}
