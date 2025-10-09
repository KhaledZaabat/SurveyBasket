using Microsoft.EntityFrameworkCore.Diagnostics;
using SurveyBasket.Domain.Common;
using System.Security.Claims;

namespace SurveyBasket.Persistence.Interceptors;

public class AuditAbleInterceptor(IHttpContextAccessor _accessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        var entries = eventData.Context.ChangeTracker.Entries<IAuditable>();
        var currentUserId = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        foreach (var entityEntry in entries)
        {

            if (entityEntry.State == EntityState.Added)
            {

                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;

            }
            else if (entityEntry.State == EntityState.Modified)
            {


                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;


            }
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        if (eventData.Context is null)
            return new ValueTask<InterceptionResult<int>>(result);

        var entries = eventData.Context.ChangeTracker.Entries<IAuditable>();
        var currentUserId = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        foreach (var entityEntry in entries)
        {

            if (entityEntry.State == EntityState.Added)
            {

                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;

            }
            else if (entityEntry.State == EntityState.Modified)
            {


                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;


            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

