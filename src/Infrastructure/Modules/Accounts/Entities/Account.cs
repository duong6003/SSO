using Core.Bases;
using Infrastructure.Modules.Applications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Accounts.Entities;

public class Account : BaseEntity
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? IdentityCard { get; set; }
    public string? Avatar { get; set; }
    public string? UserName { get; set; }
    [JsonIgnore]
    public string? Password { get; set; }
    public string? ApplicationId { get; set; }
    public DateTime? LastAccess { get; set; }
    [EnumDataType(typeof(AccountStatus))]
    public AccountStatus? AccountStatus { get; set; } = Entities.AccountStatus.Active;
    [EnumDataType(typeof(AccountType))]
    public AccountType? AccountType { get; set; }
    [EnumDataType(typeof(TwoFactorAuth))]
    public TwoFactorAuth? TwoFactorAuth { get; set; } = Entities.TwoFactorAuth.Off;
    public List<AccountPermission>? AccountPermissions { get; set; }
    public List<Application>? Applications { get; set; }
    [NotMapped]
    public string? OTPCode { get; set; }
}
public enum AccountStatus
{
    Active = 1,
    Locked = 2,
}
public enum AccountType
{
    SSO = 1,
    Normal = 2
}
public enum TwoFactorAuth
{
    On = 1,
    Off = 2
}

public class AccountConfigurations : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entityTypeBuilder)
    {
        entityTypeBuilder.HasKey(x => x.Id);
    }
}


