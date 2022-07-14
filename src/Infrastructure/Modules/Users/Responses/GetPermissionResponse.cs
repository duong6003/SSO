using Infrastructure.Modules.Users.Entities;

namespace Infrastructure.Modules.Users
{
    public class GetPermissionResponse
    {
        public string? Group { get; set; }
        public List<Permission>? Permissions { get; set; }
    }
}