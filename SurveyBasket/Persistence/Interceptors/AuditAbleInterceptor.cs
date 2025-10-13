using Microsoft.EntityFrameworkCore.Diagnostics;
using SurveyBasket.Domain.Common;

namespace SurveyBasket.Persistence.Interceptors;

public class AuditAbleInterceptor(IHttpContextAccessor _accessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not AppDbContext context)
            return result;

        if (context.DisableAuditing)
            return result;

        ApplyAuditing(eventData);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not AppDbContext context)
            return new(result);

        if (context.DisableAuditing)
            return new(result);

        ApplyAuditing(eventData);
        return new(result);
    }

    private void ApplyAuditing(DbContextEventData eventData)
    {
        var entries = eventData.Context.ChangeTracker.Entries<IAuditable>();
        var currentUserId = _accessor.HttpContext.User.GetUserId();
        foreach (var entityEntry in entries)
        {

            if (entityEntry.State == EntityState.Added)
            {

                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;

            }
            else if (entityEntry.State == EntityState.Modified && entityEntry is ISoftDeletable softDeletable && !softDeletable.IsDeleted)
            {


                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;


            }
        }
    }
}
