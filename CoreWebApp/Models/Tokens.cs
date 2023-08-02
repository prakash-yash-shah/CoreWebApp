using System.ComponentModel.DataAnnotations;

namespace CoreWebApp.Models
{
    public class Tokens
    {
        [Key]
        public int TokenId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
