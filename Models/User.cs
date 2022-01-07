using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatersTicketingAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(20,ErrorMessage = "The Username must contain between 3 and 20 characters.")]
        [MinLength(3,ErrorMessage = "The Username must contain between 3 and 20 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(20,ErrorMessage = "The Password must contain between 3 and 20 characters.")]
        [MinLength(3,ErrorMessage = "The Password must contain between 3 and 20 characters.")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}