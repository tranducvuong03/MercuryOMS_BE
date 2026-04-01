using MercuryOMS.Domain.Commons;
using Microsoft.AspNetCore.Identity;

namespace MercuryOMS.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IAuditableUser
    {
        public string FullName { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
