namespace SurveyBasket.Consts;

public static class DefaultRoles
{
    public const string Admin = "Admin";
    public const string Member = "Member";
    public static IReadOnlyList<string> GetAll()
    {
        return typeof(DefaultRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)!.ToString()!)
            .ToList();
    }
}



