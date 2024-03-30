// using System.Linq.Expressions;
// using System.Reflection.PortableExecutable;
// using Microsoft.AspNetCore.Mvc.Routing;
// using Microsoft.EntityFrameworkCore;
// using UserService.Helpers;
// using UserService.Models;
// using UserService.Models.DTO;

// namespace UserService.Services;

// public class ClientService
// {

//     public ClientService(MainDbContext dbc, IConfiguration conf)
//     {
//         HttpClientHandler clientHandler = new()
//         {
//             ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
//         };

//         _http = new HttpClient(clientHandler);
//         _dbcontext = dbc;
//         _config = conf;

//     }
//     private readonly MainDbContext _dbcontext;
//     private readonly IConfiguration _config;
//     private readonly HttpClient _http;


//     const int pageSize = 10;

//     public async Task<Guid> Register(string username, string password)
//     {
//         var client = new Client
//         {
//             Id = new Guid(),
//             Username = username,
//             PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
//         };

//         await _dbcontext.Clients.AddAsync(client);

//         try
//         {
//             await _dbcontext.SaveChangesAsync();
//         }
//         catch
//         {
//             throw new BackendException(400, "this username is already taken");
//         }

//         return client.Id;
//     }

//     public async Task<Guid> Login(string username, string password)
//     {
//         var client = await _dbcontext.Clients.FirstOrDefaultAsync(x => x.Username == username) ??
//             throw new BackendException(401, "wrong username or password");

//         if (!BCrypt.Net.BCrypt.Verify(password, client.PasswordHash))
//             throw new BackendException(401, "wrong username or password");

//         if (client.Blocked)
//             throw new BackendException(403, "client blocked");

//         return client.Id;
//     }

//     public async Task<Page<UserDTO>> ListClients(string namePattern, int pageNumber)
//     {
//         IQueryable<Client> query;
//         if (namePattern == "" || namePattern == null)
//             query = _dbcontext.Clients.Where(c => !c.Blocked).AsQueryable();
//         else
//             query = _dbcontext.Clients.Where(c => c.Username.Contains(namePattern) && !c.Blocked).AsQueryable();

//         var count = await query.CountAsync();

//         if (pageNumber < 1)
//             throw new BackendException(400, "page number can not be less than 1");

//         var clients = await query.Skip(pageSize * (pageNumber - 1))
//             .Take(pageSize)
//             .Select(x => new UserDTO { Id = x.Id, Username = x.Username })
//             .ToListAsync();

//         return new Page<UserDTO>
//         {
//             Items = clients,
//             PageInfo = new PageInfo(pageNumber, count, pageSize)
//         };
//     }

//     public async Task<UserDTO> ClientInfo(Guid id)
//     {
//         var client = await _dbcontext.Clients.FindAsync(id) ??
//             throw new BackendException(404, "client with given id does not exist");

//         return new UserDTO { Id = client.Id, Username = client.Username };
//     }

//     private async Task RollbackCoreBlock(Guid id)
//     {
//         string coreBaseUrl = _config.GetConnectionString("CoreApplication") ??
//             throw new BackendException(500, "misconfigured application");
//         var resp = await _http.PostAsync(coreBaseUrl + "User/Unblock/" + id.ToString(), null);
//         resp.EnsureSuccessStatusCode();
//     }

//     public async Task BlockClient(Guid id)
//     {

//         var client = await _dbcontext.Clients.FindAsync(id) ??
//             throw new BackendException(404, "client with given id does not exist");

//         string coreBaseUrl = _config.GetConnectionString("CoreApplication") ??
//             throw new BackendException(500, "misconfigured application");
//         string creditBaseUrl = _config.GetConnectionString("CreditApplication") ??
//             throw new BackendException(500, "misconfigured application");

//         HttpResponseMessage resp;
//         try
//         {
//             resp = await _http.PostAsync(coreBaseUrl + "User/Block/" + id.ToString(), null);
//             resp.EnsureSuccessStatusCode();
//         }
//         catch
//         {
//             throw new BackendException(500, "internal block request failed");
//         }

//         try
//         {
//             resp = await _http.PostAsync(creditBaseUrl + "User/Block/" + id.ToString(), null);
//             resp.EnsureSuccessStatusCode();
//         }
//         catch
//         {
//             await RollbackCoreBlock(id);
//             throw new BackendException(500, "internal block request failed");
//         }



//         client.Blocked = true;
//         await _dbcontext.SaveChangesAsync();
//     }
// }