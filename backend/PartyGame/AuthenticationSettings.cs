namespace PartyGame
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public int JwtExpireGame { get; set; }
        public int JwtExpireAccount { get; set; }
        public int JwtExpireRefreshAccount { get; set; }
        public string JwtIssuer { get; set; }
    }
}
