using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatersTicketingAPI.Models;

[Table("Ticket")]
public class Ticket
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "Ticket Title is required")]
    [MaxLength(60, ErrorMessage = "Ticket Title must contain between 3 and 60 characters.")]
    [MinLength(3, ErrorMessage = "Ticket Title must contain between 3 and 60 characters.")]
    public string Title { get; set; }

    [MaxLength(512, ErrorMessage = "The Ticket Description can contain up to 512 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "The Ticket Price is required")]
    [Range(1, int.MaxValue, ErrorMessage = "The Ticket price must be greater than zero")]
    public decimal Price { get; set; }

    public int UserId { get; set; }
    public User CreatedBy { get; set; }
}