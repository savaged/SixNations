namespace SixNations.Server.Models
{
    public class Token : ModelBase
    {
        public Token(
            string tokenType,
            int expiresIn,
            string accessToken)
        {
            token_type = tokenType;
            expires_in = expiresIn;
            access_token = accessToken;
        }

        public string access_token { get; }

        public int expires_in { get; }

        public string token_type { get;  }
    }
}
