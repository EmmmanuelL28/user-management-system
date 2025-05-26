using Microsoft.AspNetCore.Identity;
using System;

namespace UserManagement.Api.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        // Heredados de IdentityUser<int>, aquí los exponemos explícitamente:
        public override int Id { get; set; }
        public override string UserName { get; set; }
        public override string Email { get; set; }
        public override bool EmailConfirmed { get; set; }
        public override string PasswordHash { get; set; }
        public override string SecurityStamp { get; set; }
        public override string ConcurrencyStamp { get; set; }
        public override string PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }
        public override bool TwoFactorEnabled { get; set; }
        public override DateTimeOffset? LockoutEnd { get; set; }
        public override bool LockoutEnabled { get; set; }
        public override int AccessFailedCount { get; set; }

        // Campos de perfil
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Control de estado
        public bool IsActive { get; set; } = true;
    }
}