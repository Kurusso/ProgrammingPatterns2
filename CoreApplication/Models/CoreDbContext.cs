using Common.Extensions;
using Common.Helpers;
using Common.Models;
using CoreApplication.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using CoreApplication.Helpers;
using System.Net.Http;
namespace CoreApplication.Models
{
    public class CoreDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options){ }

        private readonly IHubContext<ClientOperationsHub> _hubContext;
        private readonly CustomWebSocketManager _customWebSocketManager;
        private readonly HttpClient _firebaseClient;
        private readonly IConfiguration _configuration;
        public CoreDbContext(DbContextOptions<CoreDbContext> options, IHubContext<ClientOperationsHub> hubContext, CustomWebSocketManager webSocketManager, IConfiguration configuration)
        : base(options)
        {
            _hubContext = hubContext;
            _customWebSocketManager = webSocketManager;
            _firebaseClient = new HttpClient();
            var firebase = configuration.GetSection("Firebase");
            _firebaseClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={firebase.GetValue<string>("ServerKey")}");
            _firebaseClient.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={firebase.GetValue<string>("SenderId")}");
            _configuration=configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.ApplyMoneyValueConverter();
            builder.ApplyGlobalFilters<IBaseEntity>(e => e.DeleteDateTime == null);
            builder.Entity<Account>().HasMany(m=>m.Operations).WithOne(t=>t.Account).HasForeignKey(t=>t.AccountId);
        }

        public override int SaveChanges()
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);

            return base.SaveChanges();
        }

        public int SaveChanges(DateTime dateTime)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess, DateTime dateTime)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
             OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker);
            await OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);
            return await  base.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(DateTime dateTime, CancellationToken cancellationToken = default)
        {
            BaseEntityTimestampHelper.SetTimestamps(ChangeTracker, dateTime);
            await OperationUpdateHelper.CatchOperationUpdate(ChangeTracker, _hubContext, this, _customWebSocketManager, _firebaseClient, _configuration);
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
