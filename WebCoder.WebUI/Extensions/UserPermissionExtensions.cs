using WebCoder.WebUI.Enums;

namespace WebCoder.WebUI.Extensions;

public static class UserPermissionExtensions
{
    public static bool HasEditPermission(this UserPermission permission)
    {
        return permission switch
        {
            UserPermission.ViewOnly => false,
            UserPermission.ViewAndUpdate => true,
            UserPermission.OwnerPermission => true,
            _ => false
        };
    }

    public static bool HasDeletePermission(this UserPermission permission)
    {
        return permission switch
        {
            UserPermission.ViewOnly => false,
            UserPermission.ViewAndUpdate => false,
            UserPermission.OwnerPermission => true,
            _ => false
        };
    }
}