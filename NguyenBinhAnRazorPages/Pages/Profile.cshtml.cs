using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages
{
    public class ProfileModel : BasePageModel
    {
        private readonly IAccountService _accountService;
        private readonly INewsArticleService _newsService;

        public ProfileModel(IAccountService accountService, INewsArticleService newsService)
        {
            _accountService = accountService;
            _newsService = newsService;
        }

        public SystemAccount? Account { get; set; }
        public int NewsCount { get; set; }

        public async Task OnGetAsync()
        {
            if (!AccountId.HasValue)
            {
                Response.Redirect("/Login");
                return;
            }

            // Load account information
            Account = await _accountService.GetAccountByIdAsync((short)AccountId.Value);
            
            // Load news count for staff users
            if (IsStaff)
            {
                var newsHistory = await _newsService.GetNewsHistoryAsync((short)AccountId.Value);
                NewsCount = newsHistory.Count();
            }
        }

        public async Task<JsonResult> OnPostUpdateAsync(string AccountName, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (!AccountId.HasValue)
                {
                    return new JsonResult(new { success = false, message = "User not authenticated" });
                }

                var account = await _accountService.GetAccountByIdAsync((short)AccountId.Value);
                if (account == null)
                {
                    return new JsonResult(new { success = false, message = "Account not found" });
                }

                // Update name
                account.AccountName = AccountName;

                // Update password if provided
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    if (string.IsNullOrEmpty(CurrentPassword))
                    {
                        return new JsonResult(new { success = false, message = "Current password is required to change password" });
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        return new JsonResult(new { success = false, message = "New passwords do not match" });
                    }

                    // Verify current password (for admin account, check against appsettings)
                    if (AccountId.Value == 0) // Admin account
                    {
                        var adminPassword = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["AdminAccount:Password"];
                        if (CurrentPassword != adminPassword)
                        {
                            return new JsonResult(new { success = false, message = "Current password is incorrect" });
                        }
                    }
                    else
                    {
                        // Verify current password for regular accounts
                        var verifyAccount = await _accountService.AuthenticateAsync(account.AccountEmail!, CurrentPassword);
                        if (verifyAccount == null)
                        {
                            return new JsonResult(new { success = false, message = "Current password is incorrect" });
                        }
                    }

                    account.AccountPassword = NewPassword;
                }

                await _accountService.UpdateAccountAsync(account);
                return new JsonResult(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}
