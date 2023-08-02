using System.ComponentModel.DataAnnotations;

namespace CoreWebApp.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }    
        public string Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string OfficeName{ get; set; }
        public bool ActiveStatus { get; set; }

    }
}
