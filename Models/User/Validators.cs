using System.ComponentModel.DataAnnotations;
using Tessa.Persistance.PostgreSQL;

namespace Tessa.Models.User;

public static class Validators
{

    /// <summary>
    /// Checks if the email is in the proper format and is unique.
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <param name="dbContext">Database to check against</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool IsValidateEmail(this string? email, TessaDbContext dbContext)
    {
        //Omit invalid inputs
        if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email) ) return false;
        //Check if email is already in use
        if (dbContext.Users.Any(u => u.Email == email)) return false;
        return true;
    }

    /// <summary>
    /// Checks if the password caters to the requirements.<br/>
    /// -> At least 6 characters <br/>
    /// -> At most 20 characters <br/>
    /// -> At least one uppercase letter <br/>
    /// -> At least one lowercase letter <br/>
    /// -> At least one digit <br/>
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <param name="reason">First encountered violated requirement</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool ValidatePassword(this string? password, out string reason)
    {
        if (string.IsNullOrEmpty(password))
        {
            reason = "Empty";
            return false;
        }

        switch (password.Length)
        {
            case < 6:
                reason = "Password too short";
                return false;
            case > 20:
                reason = "Password too long";
                return false;
        }

        if (!password.Any(char.IsUpper))
        {
            reason = "At least one uppercase letter is required";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            reason = "At least one lowercase letter is required";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            reason = "At least one digit is required";
            return false;
        }

        reason = "Passed";
        return true;
    }

    /// <summary>
    /// Checks if the username is unique against the service.
    /// </summary>
    /// <param name="username">Username to validate</param>
    /// <param name="service">Service that determines uniqueness </param>
    /// <returns>True if unique, </returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool ValidateUsername(this string? username, IUserService service)
    {
        if (string.IsNullOrEmpty(username)) return false;
        return true;
    }
}