using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages.Accounts
{
    public class IndexModel : BasePageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IEnumerable<SystemAccount> Accounts { get; set; } = new List<SystemAccount>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string searchTerm = "")
        {
            SearchTerm = searchTerm ?? string.Empty;
            
            // Load all accounts
            Accounts = await _accountService.GetAllAccountsAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Accounts = Accounts.Where(a => 
                    a.AccountName!.Contains(SearchTerm) || 
                    a.AccountEmail!.Contains(SearchTerm));
            }
        }

        public async Task<JsonResult> OnPostCreateAsync(SystemAccount account)
        {
            try
            {
                if (!IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                // Check if email already exists
                if (await _accountService.EmailExistsAsync(account.AccountEmail!))
                {
                    return new JsonResult(new { success = false, message = "Email already exists" });
                }

                // Validate role (cannot create admin through this interface)
                if (account.AccountRole == 0)
                {
                    return new JsonResult(new { success = false, message = "Cannot create admin accounts through this interface" });
                }

                var createdAccount = await _accountService.CreateAccountAsync(account);
                return new JsonResult(new { success = true, message = "Account created successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostUpdateAsync(SystemAccount account)
        {
            try
            {
                if (!IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                // Get existing account to preserve password if not changed
                var existingAccount = await _accountService.GetAccountByIdAsync(account.AccountId);
                if (existingAccount == null)
                {
                    return new JsonResult(new { success = false, message = "Account not found" });
                }

                // Check if email is being changed and if new email already exists
                if (existingAccount.AccountEmail != account.AccountEmail && 
                    await _accountService.EmailExistsAsync(account.AccountEmail!))
                {
                    return new JsonResult(new { success = false, message = "Email already exists" });
                }

                // Preserve password if empty
                if (string.IsNullOrEmpty(account.AccountPassword))
                {
                    account.AccountPassword = existingAccount.AccountPassword;
                }

                // Validate role (cannot change to admin through this interface)
                if (account.AccountRole == 0)
                {
                    return new JsonResult(new { success = false, message = "Cannot set admin role through this interface" });
                }

                await _accountService.UpdateAccountAsync(account);
                return new JsonResult(new { success = true, message = "Account updated successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(short id)
        {
            try
            {
                if (!IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return new JsonResult(new { success = false, message = "Account not found" });
                }

                // Prevent deletion of admin accounts
                if (account.AccountRole == 0)
                {
                    return new JsonResult(new { success = false, message = "Cannot delete admin accounts" });
                }

                var result = await _accountService.DeleteAccountAsync(id);
                if (result)
                {
                    return new JsonResult(new { success = true, message = "Account deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to delete account" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
