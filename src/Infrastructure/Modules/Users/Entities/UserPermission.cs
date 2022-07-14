using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Users.Entities
{
    public class UserPermission
    {
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? Code { get; set; }

        [ForeignKey("Code")]
        public Permission? Permission { get; set; }
    }
     public class UserPermissionConfigurations : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => new { x.UserId, x.Code });
        }
    }
}