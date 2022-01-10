namespace WatersTicketingAPI.DTO.Ticket
{
    public class TicketDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string CreatedBy { get; set; }
    }
}
