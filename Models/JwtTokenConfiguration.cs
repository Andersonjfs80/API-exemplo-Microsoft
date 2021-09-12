namespace ContosoPizza.Models
{
    public sealed class JwtTokenConfiguration
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpirationInSeconds { get; set; }
    }
}