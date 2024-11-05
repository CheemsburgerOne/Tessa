namespace Tessa.Models.User.ResultTypes;

public enum RegisterUserResultType
{
    Null,
    EmailNotUniqueOrInvalid,
    PasswordRequirements,
    UsernameNotUniqueOrInvalid,
    DriveError,
    Ok
}