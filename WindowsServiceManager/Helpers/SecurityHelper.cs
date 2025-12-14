using System.Security.Principal;

namespace WindowsServiceManager.Helpers;

/// <summary>
/// Provides security-related utility methods.
/// </summary>
public static class SecurityHelper
{
    /// <summary>
    /// Determines whether the current process is running with administrator privileges.
    /// </summary>
    /// <returns>True if running as administrator; otherwise, false.</returns>
    public static bool IsAdministrator()
    {
        try
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            // If we can't determine, assume not administrator
            return false;
        }
    }

    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    /// <returns>The current user name, or "Unknown" if unable to determine.</returns>
    public static string GetCurrentUserName()
    {
        try
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return identity.Name;
        }
        catch
        {
            return "Unknown";
        }
    }
}
