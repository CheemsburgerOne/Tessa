using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Tessa.Models.User.ResultTypes;
using Tessa.Persistance.PostgreSQL;
using Tessa.Utilities.IconManager;
using Tessa.Utilities.PathHelper;
using Tessa.Utilities.Result;
using Tessa.Utilities.User.UserFolderFactory;
using Directory = Tessa.Models.Filesystem.Directory.Directory;

namespace Tessa.Models.User;

public class UserService: IUserService
{
    private readonly TessaDbContext _context;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IIconManager _iconManager;
    private readonly UserFolderFactory _userFolderFactory;
    // ReSharper disable once ConvertToPrimaryConstructor
    public UserService(TessaDbContext dbContext, IHttpContextAccessor httpContext, IIconManager iconManager, UserFolderFactory userFolderFactory)
    {
        _context = dbContext;
        _httpContext = httpContext;
        _iconManager = iconManager;
        _userFolderFactory = userFolderFactory; 
    }

    /// <summary>
    /// Creates a new user based on the provided data<br/>
    /// If data is valid, creates an entry in the database<br/>
    /// </summary>
    /// <param name="dto">Data supplied with the form</param>
    /// <param name="userType">User type for the entry</param>
    /// <returns>Result of the operation. Result is True on success. <br/>
    /// On failure contains data about first not passed condition. </returns>
    public async Task<Result<RegisterUserResultType>> RegisterAsync(UserEditDto? dto, UserType? userType)
    {
        if (dto == null || userType == null)
        {
            return new Result<RegisterUserResultType>(
                RegisterUserResultType.Null
            );
        }
        
        // Validate all user credentials based on rules
        #region Validation
        
        if ( ! ( await IsEmailUniqueAsync(dto.Email) ) )
        {
            return new Result<RegisterUserResultType>( 
                RegisterUserResultType.EmailNotUniqueOrInvalid
            );
        }

        if (!dto.Password.ValidatePassword(out string _))
        {
            return new Result<RegisterUserResultType>( 
                RegisterUserResultType.PasswordRequirements
            );
        }

        if (! await IsUsernameUniqueAsync(dto.Username) )
        {
            return new Result<RegisterUserResultType>( 
                RegisterUserResultType.UsernameNotUniqueOrInvalid
            );
        }

        #endregion
        
        User newUser = new User()
        {
            Email = dto.Email,
            Username = dto.Username,
            Password = dto.Password,
            Type = UserType.Default
        };

        if (!await _userFolderFactory.CreateUserDataFolder(newUser.Username!))
        {
            return new Result<RegisterUserResultType>( 
                RegisterUserResultType.DriveError
            );
        }

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        Directory? directory = await _userFolderFactory.CreateUserRootFolder(newUser.Username!, newUser.Id);
        _context.Items.Add(directory);
        await _context.SaveChangesAsync();
        return new Result<RegisterUserResultType>(
            RegisterUserResultType.Ok
        );
        
    }
    /// <summary>
    /// Signs in the user if the credentials are correct.<br/>
    /// Repeated calls overwrite previous claims principal.<br/>
    /// </summary>
    /// <param name="dto">Data supplied with form</param>
    /// <returns>Result of the operation. Result is True on success. <br/>
    /// On failure contains data about first not passed condition.</returns>
    public async Task<Result<LoginUserResultType>> LoginAsync(LoginDto? dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
        {
            return new Result<LoginUserResultType>(
                LoginUserResultType.Null
            );
        }
        // Validate all login credentials based on rules before db query
        if (!dto.Username.NameHasValidChars())
        {
            return new Result<LoginUserResultType>(
                LoginUserResultType.InvalidUsernameChars
            );
        }

        if (dto.Password.Length is < 6 or > 20)
        {
            return new Result<LoginUserResultType>(
                LoginUserResultType.PasswordLength
            );
        }

        User? fetched;
        try
        { 
            fetched = await _context.Users
                .Where(e => e.Username == dto.Username && e.Password == dto.Password)
                .FirstAsync();
        }
        catch(InvalidOperationException)
        {
            return new Result<LoginUserResultType>(
                LoginUserResultType.NoMatch
            );
        }

        Claim[] claims = new Claim[] 
        {
            new Claim("UserGuid", fetched.Id.ToString()),
            new Claim("Username", fetched.Username!),
            new Claim("Email", fetched.Email!),
            new Claim("Usertype", fetched.Type.ToString())
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await _httpContext.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
        
        return new Result<LoginUserResultType>(
            LoginUserResultType.Ok
        );
    }
    
    /// <summary>
    /// Removes user claims from HTTP Context
    /// </summary>
    public async Task LogoutAsync()
    {
        await _httpContext.HttpContext!.SignOutAsync();
    }
    
    public async Task<User?> GetUserFromHttpContext()
    {
        ClaimsPrincipal claims = _httpContext.HttpContext!.User;
        if (claims.Identity!.IsAuthenticated)
        {
            Claim? claim = claims.FindFirst("UserGuid");
            if (claim == null) return null;
            if (!Guid.TryParse(claim.Value, out Guid guid)) return null;

            return await _context.Users.FindAsync(guid);
        }
        return null;
    }
    /// <summary>
    /// Checks if the email is in the proper format and is unique.
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <returns>True if valid, otherwise false.</returns>
    public async Task<bool> IsEmailUniqueAsync(string? email)
    {
        if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email) ) return false;
        return  !await _context.Users.AnyAsync(e => e.Email == email);
    }
    
    /// <summary>
    /// Checks if the username is in the proper format and is unique.
    /// </summary>
    /// <param name="username">Username to validate</param>
    /// <returns>True if valid, otherwise false</returns>
    public async Task<bool> IsUsernameUniqueAsync(string? username)
    {
        if (string.IsNullOrEmpty(username) && !username!.NameHasValidChars()) return false;
        return !await _context.Users.AnyAsync(e => e.Username == username);
    }
    
    /// <summary>
    /// Prepares data for NavigationBar Razor component for calling HTTP context. 
    /// </summary>
    /// <returns>DTO containing all necessary information</returns>
    public async Task<Result<UserNavMenuMiniDto>> GetNavMenuMiniDtoAsync()
    {
        User? fetched = await GetUserFromHttpContext();
        if (fetched == null) return new Result<UserNavMenuMiniDto>(
            new UserNavMenuMiniDto()
            {
                Username = "Fetch failed",
                Email = "Fetch failed",
                FullBase64Icon = _iconManager.GetUserIcon("default").FullBase64
            });
        
        UserNavMenuMiniDto dto = new UserNavMenuMiniDto()
        {
            Username = fetched.Username!,
            Email = fetched.Email!,
            FullBase64Icon = _iconManager.GetUserIcon(fetched.Username!).FullBase64
        };

        return new Result<UserNavMenuMiniDto>(dto);
    }
     
}


