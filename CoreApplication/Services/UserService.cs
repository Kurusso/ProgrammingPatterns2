using Common.Models;
using CoreApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IUserService
    {
        public Task<List<Guid>> GetBlockedUsers(); 
        public Task BlockUser(Guid userId);
        public Task UnblockUser(Guid userId);
        public Task AddNotificationsToDevice(Guid userId, string deviceToken, string appId);
    }
    public class UserService : IUserService
    {
        private readonly CoreDbContext _dbContext;
        public UserService(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Guid>> GetBlockedUsers()
        {
           return await _dbContext.BlockedUsers.Select(x=>x.UserId).ToListAsync();
        }

        public async Task BlockUser(Guid userId)
        {
            var blockedUser = new BlockedUser { UserId = userId };
            await _dbContext.AddAsync(blockedUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnblockUser(Guid userId)
        {
           var user = await _dbContext.BlockedUsers.FirstOrDefaultAsync(x=>x.UserId==userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with {userId} id is not blocked");
            }
            _dbContext.BlockedUsers.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddNotificationsToDevice(Guid userId, string deviceToken, string appId)
        {
            var token = await _dbContext.DeviceTokens.FirstOrDefaultAsync(x=>x.UserId==userId && x.Token == deviceToken);
            if (token != null)
            {
                throw new InvalidOperationException($"Notifications are on for this user {userId} for this device {deviceToken}!");
            }
            await _dbContext.DeviceTokens.AddAsync(new DeviceToken
            {
                Id = Guid.NewGuid(),
                Token = deviceToken,
                UserId = userId,
                AppId = appId
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
