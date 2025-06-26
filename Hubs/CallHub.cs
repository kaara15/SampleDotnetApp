using Microsoft.AspNetCore.SignalR;
using SampleDotnetApp.Models.DataModels;
using SampleDotnetApp.Services.Interfaces;
using SampleDotnetApp.Utilities.Interfaces;

namespace SampleDotnetApp.Hubs
{
    public class CallHub : Hub
    {
        private readonly IDataCacheService<DataCacheDataModel> dataCacheService;

        public CallHub(ICacheFactory<DataCacheDataModel> cacheFactory)
        {
            dataCacheService = cacheFactory.GetDefaultCache();
        }
        
        private static readonly Dictionary<string, string> ConnectedUsers = new();

        public override Task OnConnectedAsync()
        {
            string email = Context.User?.Identity?.Name ?? Context.ConnectionId;
            ConnectedUsers[Context.ConnectionId] = email;
            List<ConnectionUserDataModel> connectedUsersList = dataCacheService.GetFromCache(ConnectedUsers.Values.ToList()).Select(user => new ConnectionUserDataModel
            {
                Email = user.Value.Email,
                FullName = user.Value.FullName
            }).ToList();
            Clients.All.SendAsync("UserListUpdated", connectedUsersList);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers.Remove(Context.ConnectionId);
            List<ConnectionUserDataModel> connectedUsersList = dataCacheService.GetFromCache(ConnectedUsers.Values.ToList()).Select(user => new ConnectionUserDataModel
            {
                Email = user.Value.Email,
                FullName = user.Value.FullName
            }).ToList();
            Clients.All.SendAsync("UserListUpdated", connectedUsersList);
            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionIdForUser(string email)
        {
            return ConnectedUsers.FirstOrDefault(user => string.Equals(user.Value, email, StringComparison.OrdinalIgnoreCase)).Key ?? string.Empty;
        }
    }
}