using Microsoft.AspNetCore.SignalR;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Hubs
{
    public class NewsHub : Hub
    {
        public async Task NotifyNewsCreated(NewsArticle newsArticle)
        {
            await Clients.All.SendAsync("NewsCreated", newsArticle);
        }

        public async Task NotifyNewsUpdated(NewsArticle newsArticle)
        {
            await Clients.All.SendAsync("NewsUpdated", newsArticle);
        }

        public async Task NotifyNewsDeleted(string newsArticleId)
        {
            await Clients.All.SendAsync("NewsDeleted", newsArticleId);
        }

        public async Task NotifyCategoryCreated(Category category)
        {
            await Clients.All.SendAsync("CategoryCreated", category);
        }

        public async Task NotifyCategoryUpdated(Category category)
        {
            await Clients.All.SendAsync("CategoryUpdated", category);
        }

        public async Task NotifyCategoryDeleted(short categoryId)
        {
            await Clients.All.SendAsync("CategoryDeleted", categoryId);
        }

        public async Task NotifyAccountCreated(SystemAccount account)
        {
            await Clients.All.SendAsync("AccountCreated", account);
        }

        public async Task NotifyAccountUpdated(SystemAccount account)
        {
            await Clients.All.SendAsync("AccountUpdated", account);
        }

        public async Task NotifyAccountDeleted(short accountId)
        {
            await Clients.All.SendAsync("AccountDeleted", accountId);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
