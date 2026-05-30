namespace ApiMiniPrj.Domain.Constants
{
    public static class Permissions
    {
        public static class User
        {
            public const string View = "Permissions.User.View";
            public const string Create = "Permissions.User.Create";
            public const string Edit = "Permissions.User.Edit";
            public const string Delete = "Permissions.User.Delete";
        }

        public static class Role
        {
            public const string View = "Permissions.Role.View";
            public const string Manage = "Permissions.Role.Manage";
        }

        public static class Events
        {
            public const string View = "Permissions.Events.View";
            public const string Create = "Permissions.Events.Create";
            public const string Edit = "Permissions.Events.Edit";
            public const string Delete = "Permissions.Events.Delete";
            public const string AddBanner = "Permissions.Events.AddBanner";
        }

        public static class Organizers
        {
            public const string View = "Permissions.Organizers.View";
            public const string Create = "Permissions.Organizers.Create";
            public const string Edit = "Permissions.Organizers.Edit";
            public const string Delete = "Permissions.Organizers.Delete";
            public const string AddLogo = "Permissions.Organizers.AddLogo";
        }

        public static class Tickets
        {
            public const string View = "Permissions.Tickets.View";
            public const string Create = "Permissions.Tickets.Create";
            public const string Edit = "Permissions.Tickets.Edit";
            public const string Delete = "Permissions.Tickets.Delete";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        public static IEnumerable<string> All()
        {
            yield return User.View;
            yield return User.Create;
            yield return User.Edit;
            yield return User.Delete;
            yield return Role.View;
            yield return Role.Manage;
            yield return Events.View;
            yield return Events.Create;
            yield return Events.Edit;
            yield return Events.Delete;
            yield return Events.AddBanner;
            yield return Organizers.View;
            yield return Organizers.Create;
            yield return Organizers.Edit;
            yield return Organizers.Delete;
            yield return Organizers.AddLogo;
            yield return Tickets.View;
            yield return Tickets.Create;
            yield return Tickets.Edit;
            yield return Tickets.Delete;
            yield return Users.View;
            yield return Users.Edit;
            yield return Users.Delete;
            yield return Users.Create;

        }
    }
}
