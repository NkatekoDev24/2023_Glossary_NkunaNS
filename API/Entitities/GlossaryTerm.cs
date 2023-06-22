using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entitities
{
    [Table("GlossaryTerm")]
    public class GlossaryTerm
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Term { get; set; }
        public string Definition { get; set; }
        public string UserName { get; set; } // Foreign key property

        public AppUser User { get; set; } // Navigation property
    }
}