using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using System.ComponentModel.DataAnnotations;

namespace NguyenBinhAnRazorPages.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public LoginModel(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if it's the admin account from appsettings
                var adminEmail = _configuration["AdminAccount:Email"];
                var adminPassword = _configuration["AdminAccount:Password"];

                if (Email == adminEmail && Password == adminPassword)
                {
                    // Create admin session
                    HttpContext.Session.SetString("UserEmail", adminEmail);
                    HttpContext.Session.SetString("UserName", "Administrator");
                    HttpContext.Session.SetInt32("AccountId", 0); // Special ID for admin
                    HttpContext.Session.SetInt32("AccountRole", 0); // 0 for Admin
                    return RedirectToPage("/Index");
                }

                // Check regular accounts
                var account = await _accountService.AuthenticateAsync(Email, Password);
                
                if (account != null)
                {
                    // Create user session
                    HttpContext.Session.SetString("UserEmail", account.AccountEmail ?? string.Empty);
                    HttpContext.Session.SetString("UserName", account.AccountName ?? string.Empty);
                    HttpContext.Session.SetInt32("AccountId", account.AccountId);
                    HttpContext.Session.SetInt32("AccountRole", account.AccountRole ?? 0);
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login: {ex.Message}";
            }

            return Page();
        }
    }
}
