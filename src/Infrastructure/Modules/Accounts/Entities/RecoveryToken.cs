namespace Infrastructure.Modules.Accounts.Entities
{
    public class RecoveryToken
    {
        public string? AccountId { get; set; }
        public string? Token { get; set; }
    }
}
