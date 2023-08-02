using System.ComponentModel.DataAnnotations;

namespace JsonWebTokens.Models
{
    public class UserLogin
    {
        [Key]
        public int Loginid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        //public string Role { get; set; }
        //public string Email { get; set; }
        //public string OfficeName { get; set; }
        //public bool IsActive { get; set; }
    }
}
