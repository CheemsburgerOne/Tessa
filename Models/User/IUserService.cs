namespace Tessa.Models.User;

public interface IUserService
{
    public Task<Guid?> CreateAsync(UserEditDto dto, UserType type);
    public Task<User> GetAsync(Guid id);
    public Task<bool> LoginAsync(IHttpContextAccessor httpContext, LoginDto dto);
    public Task LogoutAsync(IHttpContextAccessor httpContext);
}