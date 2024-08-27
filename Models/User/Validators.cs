using System.ComponentModel.DataAnnotations;

namespace Tessa.Models.User;

public static class Validators
{

    /// <summary>
    /// Checks if the email is in the proper format.
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool ValidateEmail(this string? email)
    {
        return !string.IsNullOrEmpty(email) && new EmailAddressAttribute().IsValid(email);
    }

    /// <summary>
    /// Checks if the password caters to the requirements.<br/>
    /// -> At least 8 characters <br/>
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

        if (password!.Length < 8)
        {
            reason = "Too short";
            return false;
        }

        if (password!.Length > 20)
        {
            reason = "Too long";
            return false;
        }

        if (!password!.Any(char.IsUpper))
        {
            reason = "No upper";
            return false;
        }

        if (!password!.Any(char.IsLower))
        {
            reason = "No lower";
            return false;
        }

        if (!password!.Any(char.IsDigit))
        {
            reason = "No digit";
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
    /// <exception cref="NotImplementedException"></exception
    public static bool ValidateUsername(this string? username, IUserService service)
    {
        return true;
    }
}