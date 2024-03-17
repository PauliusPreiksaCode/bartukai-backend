using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace portal_backend.Helpers;

public static class AuthorizationHelpers
{
    public static bool ValidateEmail(string email)
    {
        var emailRegex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }

    public static bool ValidatePassword(string password)
    {
        if (password.Length < 8) return false;

        var containsCapitalRegex = new Regex(@"[A-Z]");
        var containsNonCapitalRegex = new Regex(@"[a-z]");
        var containsNumberRegex = new Regex(@"[0-9]");
        var containsSpecialRegex = new Regex(@"[\p{P}\p{S}]");

        if (!containsCapitalRegex.Match(password).Success) return false;
        if (!containsNonCapitalRegex.Match(password).Success) return false;
        if (!containsNumberRegex.Match(password).Success) return false;
        if (!containsSpecialRegex.Match(password).Success) return false;

        return true;
    }

    public static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using var sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array  
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}