using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Business.Repositories;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data;
using NguyenBinhAnRazorPages.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Initialize Singleton pattern: FUNewsManagementDbContextFactory
// This singleton holds the connection string and is initialized once at startup
FUNewsManagementDbContextFactory.Instance.Initialize(
    builder.Configuration.GetConnectionString("DefaultConnection")!);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure Entity Framework
builder.Services.AddDbContext<FUNewsManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IGenericRepository<NguyenBinhAn_A02_Data.Models.SystemAccount>, SystemAccountRepository>();
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<IGenericRepository<NguyenBinhAn_A02_Data.Models.Category>, CategoryRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGenericRepository<NguyenBinhAn_A02_Data.Models.NewsArticle>, NewsArticleRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<IGenericRepository<NguyenBinhAn_A02_Data.Models.Tag>, TagRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Register services
// AccountService, CategoryService, NewsArticleService use Scoped because they depend on Scoped DbContext
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
// TagService is registered as Singleton (Singleton pattern) - it only reads tag data
// and delegates all DB access to the Scoped TagRepository via IServiceScopeFactory
builder.Services.AddScoped<ITagService, TagService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.MapStaticAssets();

// Configure SignalR Hub
app.MapHub<NewsHub>("/newsHub");

// Set default route to login page
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Login");
        return;
    }
    await next();
});

app.Run();
