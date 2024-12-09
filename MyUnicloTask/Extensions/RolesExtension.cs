using Ab108Uniqlo.Views.Account.Enums;

namespace Ab108Uniqlo.Extensions;

public static class RolesExtension
{
    public static string GetRole(this Roles role)
    {
        return role switch
        {
            Roles.User => nameof(Roles.User),
            Roles.Admin => nameof(Roles.Admin),
            Roles.Moderator => nameof(Roles.Moderator),

        };

    }
}
