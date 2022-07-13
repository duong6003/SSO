using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Accounts.Entities
{
    [Table("permission")]
    public class Permission
    {
        public string? PermissionCode { get; set; }
        public string? Name { get; set; }
        public string? Group { get; set; }
    }
}
