using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly ISystemAccountRepository _accountRepository;

        public AccountService(ISystemAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
        {
            return await _accountRepository.AuthenticateAsync(email, password);
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short accountId)
        {
            return await _accountRepository.GetByIdAsync(accountId);
        }

        public async Task<SystemAccount> CreateAccountAsync(SystemAccount account)
        {
            return await _accountRepository.AddAsync(account);
        }

        public async Task<SystemAccount> UpdateAccountAsync(SystemAccount account)
        {
            await _accountRepository.UpdateAsync(account);
            return account;
        }

        public async Task<bool> DeleteAccountAsync(short accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null) return false;

            await _accountRepository.DeleteAsync(account);
            return true;
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var account = await _accountRepository.GetByEmailAsync(email);
            return account != null;
        }

        public async Task<SystemAccount?> GetAdminAccountAsync(string adminEmail, string adminPassword)
        {
            return await _accountRepository.AuthenticateAsync(adminEmail, adminPassword);
        }
    }
}
