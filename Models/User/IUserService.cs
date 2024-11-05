using Tessa.Models.User.ResultTypes;
using Tessa.Utilities.Result;

namespace Tessa.Models.User;

public interface IUserService
{
    public Task<Result<RegisterUserResultType>> RegisterAsync(UserEditDto? dto, UserType? type);
    public Task<Result<LoginUserResultType>> LoginAsync(LoginDto dto);
    public Task<User?> GetUserFromHttpContext();
    public Task LogoutAsync();
    public Task<bool> IsEmailUniqueAsync(string email);
    public Task<bool> IsUsernameUniqueAsync(string? username);
    public Task<Result<UserNavMenuMiniDto>> GetNavMenuMiniDtoAsync();
}