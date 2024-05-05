using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.DTO;

namespace UserService.Services;

public class UsersService(
    MainDbContext dbc, 
    UserManager<User> um
) {
    private readonly MainDbContext _dbcontext = dbc;
    private readonly UserManager<User> _userManager = um;
    const int pageSize = 10;

    public async Task<Guid> Register(string username, string password, IEnumerable<string> roles)
    {
        var user = new User {
            Email = username,
            UserName = username
        };
        var result =  await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new BackendException(400, $"Failed to create the user: {result}");


        result = await _userManager.AddToRolesAsync(user, roles);
        if (!result.Succeeded)
            throw new BackendException(400, $"Failed assign roles to user: {result}");

        return user.Id;
    }

    public async Task<Page<UserDTO>> ListUsers(string namePattern, int pageNumber, string role)
    {        
        var users = await _userManager.GetUsersInRoleAsync(role);
        var query = users.AsQueryable();
        if (namePattern == "" || namePattern == null)
            query = query.Where(c => !c.Blocked).AsQueryable();
        else
            query = query.Where(c => c.UserName.Contains(namePattern) && !c.Blocked).AsQueryable();

        var count = query.Count();

        if (pageNumber < 1)
            throw new BackendException(400, "page number can not be less than 1");

        var staff = query.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new UserDTO { Id = x.Id, Username = x.UserName })
            .ToList();

        return new Page<UserDTO>
        {
            Items = staff,
            PageInfo = new PageInfo(pageNumber, count, pageSize)
        };
    }

    public async Task<UserDTO> UserInfo(Guid id) {
        var user = await _userManager.FindByIdAsync(id.ToString()) ??
            throw new BackendException(404, "client with given id does not exist");

        return new UserDTO { Id = user.Id, Username = user.UserName };
    }

    public async Task BlockUser(Guid id)
    {
        User user = await _userManager.FindByIdAsync(id.ToString()) ??
            throw new BackendException(404, "client with given id does not exist");

        user.Blocked = true;

        await _dbcontext.SaveChangesAsync();
    }

    public bool CanSeeUser(ClaimsPrincipal authUser, Guid userId) {
        var id = authUser.FindFirstValue("sub");
        if (id == null)
            return false;
        
        return authUser.IsInRole(IdentityConfigurator.StaffRole) || userId.ToString() == id;       
    }
}