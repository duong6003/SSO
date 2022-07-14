using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Users.Entities
{
    public class Role : BaseEntity
    {
        public string? Name { get; set; }
        public List<User>? Users { get; set; }
        public virtual List<RolePermission>? RolePermissions { get; set; }
    }
     public class RoleConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
        }
    }
}