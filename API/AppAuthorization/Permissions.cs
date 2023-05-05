namespace API.AppAuthorization;
public static class Permissions
{
    public static class Dashboard
    {
        public const string View = "Permissions.Dashboard.View";
    }

    public static class User
    {
        public const string List = "Permissions.User.List";
        public const string View = "Permissions.User.View";
        public const string Create = "Permissions.User.Create";
        public const string Edit = "Permissions.User.Edit";
        public const string Delete = "Permissions.User.Delete";
    }
}

public class CustomClaimTypes
{
    public const string Permission = "permission";
}