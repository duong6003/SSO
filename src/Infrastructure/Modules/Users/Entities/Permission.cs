using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Modules.Users.Entities
{
    public class Permission
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Code);
        }
    }
}