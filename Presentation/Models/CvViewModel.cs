namespace Presentation.Models
{
    public class CvViewModel
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string UserEmail { get; set; }
        public required string EmployerEmail { get; set; }
    }
}
