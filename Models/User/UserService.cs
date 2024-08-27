using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tessa.Persistance.PostgreSQL;

namespace Tessa.Models.User;

public class UserService(
    TessaDbContext context
    // IEventService eventService
) : IUserService
{
    private readonly TessaDbContext _context = context;
    // private readonly IEventService _eventService = eventService;

    public async Task<Guid?> CreateAsync(UserEditDto dto, UserType userType)
    {
        // Validate all user credentials based on rules

        #region Validation

        // EventEditDto eventDto;

        if (!dto.Email.ValidateEmail())
        {
            // eventDto = new EventEditDto()
            // {
            //     Description = $"INV EMAIL {dto.Username} : {dto.Email}",
            //     EventType = EventType.Register,
            //     ErrorType = ErrorType.EmailInvalid
            // };
            //
            // await _eventService.CreateAsync(eventDto);
            await _context.SaveChangesAsync();
            return null;
        }

        if (!dto.Password.ValidatePassword(out string reason))
        {
            // eventDto = new EventEditDto()
            // {
            //     Description = $"INV PASSWD {dto.Username} : {dto.Email} REASON: {reason}",
            //     EventType = EventType.Register,
            //     ErrorType = ErrorType.PasswordInvalid
            // };
            //
            // await _eventService.CreateAsync(eventDto);
            await _context.SaveChangesAsync();
            return null;
        }

        if (!dto.Username.ValidateUsername(this))
        {
            // eventDto = new EventEditDto()
            // {
            //     Description = $"INV USR {dto.Username} : {dto.Email}",
            //     EventType = EventType.Register,
            //     ErrorType = ErrorType.UsernameInvalid
            // };
            //
            // await _eventService.CreateAsync(eventDto);
            await _context.SaveChangesAsync();
            return null;
        }

        #endregion

        User newUser = new User()
        {
            Email = dto.Email,
            Username = dto.Username,
            Password = dto.Password,
            Type = userType
        };

        EntityEntry<User> entry = await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        // eventDto = new EventEditDto()
        // {
        //     Description = $" {entry.Entity.Type.ToString()} ACC CRT {newUser.Username} : {newUser.Email}",
        //     EventType = EventType.Register,
        //     ItemId = null,
        //     UserId = newUser.Id
        // };

        // await _eventService.CreateAsync(eventDto);
        await _context.SaveChangesAsync();
        return entry.Entity.Id;
    }

    public async Task<bool> LoginAsync(IHttpContextAccessor httpContext, LoginDto dto)
    {
        User? fetched = await _context.Users.Where(e => e.Username == dto.Username && e.Password == dto.Password)
            .FirstAsync();

        Claim[] claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, fetched.Username!),
            new Claim(ClaimTypes.UserData, fetched.Email!),
            new Claim(ClaimTypes.Role, fetched.Type.ToString())
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
        return true;
    }

    public async Task LogoutAsync(IHttpContextAccessor httpContext)
    {
        await httpContext.HttpContext!.SignOutAsync();
    }

    public Task<User> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
