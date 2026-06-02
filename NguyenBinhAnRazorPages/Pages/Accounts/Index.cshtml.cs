using System;
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

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public int StartRecord { get; set; } = 1;
        public int EndRecord { get; set; } = 5;

        public async Task OnGetAsync(string searchTerm = "", int page = 1)
        {
            SearchTerm = searchTerm ?? string.Empty;
            CurrentPage = page < 1 ? 1 : page;

            // Load all accounts
            var allAccounts = (await _accountService.GetAllAccountsAsync()).OrderBy(a => a.AccountId).ToList();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allAccounts = allAccounts.Where(a =>
                    a.AccountName!.Contains(SearchTerm) ||
                    a.AccountEmail!.Contains(SearchTerm)).ToList();
            }

            TotalRecords = allAccounts.Count;
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            if (CurrentPage > TotalPages) CurrentPage = TotalPages > 0 ? TotalPages : 1;

            StartRecord = TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
            EndRecord = Math.Min(CurrentPage * PageSize, TotalRecords);

            Accounts = allAccounts.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
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
