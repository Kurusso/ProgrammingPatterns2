using Common.Extensions;
using Common.Helpers;
using Common.Models;
using CoreApplication.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using CoreApplication.Helpers;
namespace CoreApplication.Models
{
    public class CoreDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options){ }

        private readonly IHubContext<ClientOperationsHub> _hubContext;
        private readonly CustomWebSocketManager _customWebSocketManager;
        public CoreDbContext(DbContextOptions<CoreDbContext> options, IHubContext<ClientOperationsHub> hubContext, CustomWebSocketManager webSocketManager)
        : base(options)
        {
            _hubContext = hubContext;
            _customWebSocketManager = webSocketManager;
        }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.ApplyMoneyValueConverter();
            builder.Entity<Account>().HasMany(m=>m.Operations).WithOne(t=>t.Account).HasForeignKey(t=>t.AccountId);
        }

        public override int SaveChanges()
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);

            return base.SaveChanges();
        }

        public int SaveChanges(DateTime dateTime)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess, DateTime dateTime)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
             OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            await OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);
            return await  base.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(DateTime dateTime, CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
            await OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager);
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(DateTime dateTime, bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
            return await  base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
