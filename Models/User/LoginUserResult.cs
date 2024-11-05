using Tessa.Models.User.ResultTypes;

namespace Tessa.Models.User;

public struct LoginUserResult
{
    /// <summary>
    /// Indicates if the user login was successful.
    /// </summary>
    public bool Result { get; set; }
    /// <summary>
    /// Enumerates the type of value that was first caught invalid.
    /// </summary>
    public RegisterUserResultType RegisterUserResultType { get; set; }
    /// <summary>
    /// Reason for the invalidation.
    /// </summary>
    public string Reason { get; set; }
}