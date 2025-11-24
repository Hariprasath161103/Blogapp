using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Blogapp.Hubs
{
    // The CommentHub handles real-time communication related to comments.
    public class CommentHub : Hub
    {
        // This method is called by the client (home.razor) when a comment is added.
        public async Task NotifyAuthor(string authorEmail, string blogTitle)
        {
            var message = $"New comment posted on your blog: '{blogTitle}'!";

            // Broadcast the notification to all clients, allowing the receiving client 
            // to filter it based on their own UserEmail.
            await Clients.All.SendAsync("ReceiveNotification", authorEmail, message);
        }
    }
}
