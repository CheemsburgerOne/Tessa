namespace Tessa.Models.User.ResultTypes;

public enum LoginUserResultType
{
    Null,
    InvalidUsernameChars,
    PasswordLength,
    NoMatch,
    Ok
}