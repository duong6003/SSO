using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Infrastructure.Modules.Users.Entities;

public class User : BaseEntity
{
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? EmailAddress { get; set; }

    [JsonIgnore]
    public string? Password { get; set; }

    public string? Avatar { get; set; }
    public bool Active { get; set; } = true;
    public Guid? RoleId { get; set; }
    [JsonIgnore]
    public Role? Role { get; set; }
    public List<UserPermission>? UserPermissions { get; set; }
}
public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entityTypeBuilder)
    {
        entityTypeBuilder.HasKey(x => x.Id);
    }
}