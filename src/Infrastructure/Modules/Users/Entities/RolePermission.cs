using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Users.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public string? Code { get; set; }

        [ForeignKey("Code")]
        public Permission? Permission { get; set; }
    }
    public class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> entityTypeBuilder)
        {

            entityTypeBuilder.HasKey(x => new { x.RoleId, x.Code });
        }
    }
}