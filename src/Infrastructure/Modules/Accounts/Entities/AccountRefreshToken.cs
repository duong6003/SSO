using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Bases;

namespace Infrastructure.Modules.Accounts.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string? ApplicationId { get; set; }
        public string? AccountId { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
    }

}
