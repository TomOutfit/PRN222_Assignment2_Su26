using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public interface ISystemAccountRepository : IGenericRepository<SystemAccount>
    {
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount?> AuthenticateAsync(string email, string password);
        Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role);
    }
}
