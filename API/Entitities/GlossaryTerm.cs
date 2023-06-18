using Microsoft.AspNetCore.Identity;

namespace API.Entitities
{
    public class GlossaryTerm
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string UserId { get; set; } // Foreign key property

        public AppUser User { get; set; } // Navigation property
    }
}