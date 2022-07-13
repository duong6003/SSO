using Newtonsoft.Json;
using Infrastructure.Modules.Accounts.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Bases;

namespace Infrastructure.Modules.Applications.Entities
{
    [Table("application")]
    public class Application : BaseEntity
    {
        [Column("account_id")]
        public string AccountId { get; set; }
        [Column("application_name")]
        public string ApplicationName { get; set; }
        [Column("application_icon")]
        public string ApplicationIcon { get; set; }
        [JsonIgnore]
        [Column("application_secret")]
        public string ApplicationSecret { get; set; }
        [Column("application_redirect_url")]
        public string ApplicationRedirectUrl { get; set; }
        [Column("application_web_hook")]
        public string ApplicationWebHook { get; set; }
        [Column("application_status")]
        [EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Active;
        public Account Account { get; set; }
    }
    public enum ApplicationStatus : int
    {
        Active = 1,
        Locked = 0
    }
}
