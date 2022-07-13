using Core.Bases;
using Infrastructure.Modules.Accounts.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Accounts.Entities
{
    [Table("account_permission")]
    public class AccountPermission : BaseEntity
    {
        public string? PermissionCode { get; set; }
        public string? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
