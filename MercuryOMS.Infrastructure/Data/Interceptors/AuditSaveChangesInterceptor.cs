using MercuryOMS.Domain.Commons;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MercuryOMS.Infrastructure.Data.Interceptors
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;

        public AuditSaveChangesInterceptor(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableEntity &&
                            (e.State == EntityState.Added ||
                             e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var userId = _currentUser.UserId;

                if (entry.Entity is AuditableEntity entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = DateTime.UtcNow;
                    }

                    entity.LastModifiedAt = DateTime.UtcNow;
                }

                if (entry.Entity is IAuditableUser userEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (string.IsNullOrEmpty(userEntity.CreatedBy))
                            userEntity.CreatedBy = userId.ToString();
                    }

                    userEntity.LastModifiedBy = userId.ToString();
                }
            }
        }
    }
}
