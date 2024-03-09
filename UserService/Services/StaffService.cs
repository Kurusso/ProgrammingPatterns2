using Microsoft.EntityFrameworkCore;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.DTO;

namespace UserService.Services;

public class StaffService(MainDbContext dbc)
{
    private readonly MainDbContext _dbcontext = dbc;
    const int pageSize = 10;

    public async Task<Guid> Register(string username, string password)
    {
        var staff = new Staff
        {
            Id = new Guid(),
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _dbcontext.Staff.AddAsync(staff);

        try
        {
            await _dbcontext.SaveChangesAsync();
        }
        catch
        {
            throw new BackendException(400, "this username is already taken");
        }

        return staff.Id;
    }

    public async Task<Page<UserDTO>> ListStaff(string namePattern, int pageNumber)
    {
        IQueryable<Staff> query;
        if (namePattern == "" || namePattern == null)
            query = _dbcontext.Staff.Where(c => !c.Blocked).AsQueryable();
        else
            query = _dbcontext.Staff.Where(c => c.Username.Contains(namePattern) && !c.Blocked).AsQueryable();

        var count = await query.CountAsync();

        if (pageNumber < 1)
            throw new BackendException(400, "page number can not be less than 1");

        var staff = await query.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new UserDTO { Id = x.Id, Username = x.Username })
            .ToListAsync();

        return new Page<UserDTO>
        {
            Items = staff,
            PageInfo = new PageInfo(pageNumber, count, pageSize)
        };
    }

    public async Task BlockStaff(Guid id)
    {
        var staff = await _dbcontext.Staff.FindAsync(id) ??
            throw new BackendException(404, "client with given id does not exist");

        staff.Blocked = true;

        await _dbcontext.SaveChangesAsync();
    }
}