using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public interface IAccountService
    {
        Task<SystemAccount?> AuthenticateAsync(string email, string password);
        Task<SystemAccount?> GetAccountByIdAsync(short accountId);
        Task<SystemAccount> CreateAccountAsync(SystemAccount account);
        Task<SystemAccount> UpdateAccountAsync(SystemAccount account);
        Task<bool> DeleteAccountAsync(short accountId);
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<SystemAccount?> GetAdminAccountAsync(string adminEmail, string adminPassword);
    }
}
