namespace SurveyBasket.Auhtentication_Providers.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(policy: permission)
{
}
