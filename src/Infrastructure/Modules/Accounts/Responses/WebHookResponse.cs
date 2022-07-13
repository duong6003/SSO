using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Responses
{
    public class WebHookResponse
    {
        public List<string> AccountIds { get; set; }
        public ActionType ActionType { get; set; }
        public WebHookResponse(List<string> accountIds, ActionType actionType)
        {
            AccountIds = accountIds;
            ActionType = actionType;
        }
    }
    public enum ActionType : int
    {
        DeleteAccount = 0,
        ActiveAccount = 1,
        DeactiveAccount = 2
    }

}
