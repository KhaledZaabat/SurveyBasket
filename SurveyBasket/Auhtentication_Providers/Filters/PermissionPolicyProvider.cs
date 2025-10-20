using Microsoft.Extensions.Options;

namespace SurveyBasket.Auhtentication_Providers.Filters;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy=await base.GetPolicyAsync(policyName);
        if (policy is not null) return policy;
        AuthorizationPolicy permissionPolicy=new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
          options.Value.AddPolicy(policyName,permissionPolicy);
        return permissionPolicy;
    }
}