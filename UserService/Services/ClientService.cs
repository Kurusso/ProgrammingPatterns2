using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.DTO;

namespace UserService.Services;

public class ClientService(MainDbContext dbc)
{
    private readonly MainDbContext _dbcontext = dbc;
    const int pageSize = 10;

    public async Task<Guid> Register(string username, string password)
    {
        var client = new Client{
            Id = new Guid(),
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _dbcontext.Clients.AddAsync(client);
        await _dbcontext.SaveChangesAsync();
        return client.Id;
    }

    public async Task<Guid> Login(string username, string password) 
    {
        var client = await _dbcontext.Clients.FirstOrDefaultAsync(x => x.Username == username) ??
            throw new BackendException(401, "wrong username or password");
        
        if (!BCrypt.Net.BCrypt.Verify(password, client.PasswordHash))
            throw new BackendException(401, "wrong username or password");
        
        return client.Id;
    }

    public async Task<Page<UserDTO>> ListClients(string namePattern, int pageNumber) {
        IQueryable<Client> query;
        if (namePattern == "" || namePattern == null)
            query = _dbcontext.Clients.AsQueryable();
        else 
            query = _dbcontext.Clients.Where(c => c.Username.Contains(namePattern)).AsQueryable();

        var count = await query.CountAsync();

        if (pageNumber < 1) 
            throw new BackendException(400, "page number can not be less than 1");

        var clients =  await query.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new UserDTO{Id=x.Id, Username=x.Username})
            .ToListAsync();

        return new Page<UserDTO>
        {
            Items = clients,
            PageInfo = new PageInfo(pageNumber, count, pageSize)
        }; 
    }

    public async Task<UserDTO> ClientInfo(Guid id) {
        var client = await _dbcontext.Clients.FindAsync(id) ??
            throw new BackendException(404, "client with given id does not exist");

        return new UserDTO{Id=client.Id, Username=client.Username};
    }
}