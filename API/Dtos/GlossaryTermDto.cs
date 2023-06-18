namespace API.Dtos
{
    public class GlossaryTermDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
    }
}