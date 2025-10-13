using Microsoft.EntityFrameworkCore.Diagnostics;
using SurveyBasket.Domain.Common;

namespace SurveyBasket.Persistence.Interceptors;

public class SoftDeleteInterceptor(IHttpContextAccessor _accessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        if (eventData.Context is not AppDbContext context)
            return result;

        if (context.DisableSoftDeletion)
            return result;

        ApplySoftDelete(eventData);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {

        if (eventData.Context is null)
            return new ValueTask<InterceptionResult<int>>(result);


        if (eventData.Context is not AppDbContext context)
            return new ValueTask<InterceptionResult<int>>(result);

        if (context.DisableSoftDeletion)
            return new ValueTask<InterceptionResult<int>>(result);
        ApplySoftDelete(eventData);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    private void ApplySoftDelete(DbContextEventData eventData)
    {
        var entries = eventData.Context.ChangeTracker.Entries<ISoftDeletable>();
        var currentUserId = _accessor.HttpContext?.User.GetUserId();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.Property(x => x.IsDeleted).CurrentValue = true;
                entry.Property(x => x.DeletedById).CurrentValue = currentUserId;
                entry.Property(x => x.DeletedOn).CurrentValue = DateTime.UtcNow;

                entry.State = EntityState.Modified;

                foreach (var ownedEntry in entry.References
                .Where(r => r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned()))
                {
                    ownedEntry.TargetEntry!.State = EntityState.Modified;
                }
            }
        }
    }
}





