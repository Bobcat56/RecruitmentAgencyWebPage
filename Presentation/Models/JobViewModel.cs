namespace Presentation.Models
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Requirments { get; set; }
        public DateOnly PostingDate { get; set; }
        public string EmployerId { get; set; }
    }
}
