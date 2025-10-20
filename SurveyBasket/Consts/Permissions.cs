namespace SurveyBasket.Consts;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    public static class Surveys
    {
        public const string Read = "surveys:read";
        public const string Add = "surveys:add";
        public const string Update = "surveys:update";
        public const string Delete = "surveys:delete";
    }

    public static class Questions
    {
        public const string Read = "questions:read";
        public const string Add = "questions:add";
        public const string Update = "questions:update";
    }

    public static class Users
    {
        public const string Read = "users:read";
        public const string Add = "users:add";
        public const string Update = "users:update";
    }

    public static class Roles
    {
        public const string Read = "roles:read";
        public const string Add = "roles:add";
        public const string Update = "roles:update";
    }

    public static class Results
    {
        public const string Read = "results:read";
    }

 
    public static IList<string> GetAllPermissions()
    {
        var permissions = new List<string>();

        var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var type in nestedTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                        permissions.Add(value);
                }
            }
        }

        return permissions;
    }
}
