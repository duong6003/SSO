namespace Infrastructure.Definitions
{
    public static class RoutePath
    {
        public static IDictionary<string, string> UserRoutePath = new Dictionary<string, string>()
        {
            {"/api/Feedbacks", "POST"},
            {"/api/Feedbacks/FeedbackUser", "GET"},
            {"/api/Feebacks/Rating", "PUT"},
            {"/api/Feedbacks/Comment", "POST"}
        };
    }
}
