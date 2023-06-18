using Microsoft.AspNetCore.Identity;

namespace API.Entitities
{
    public class AppUser : IdentityUser
    {
        public List<GlossaryTerm> GlossaryTerms { get; set; } 
    }
}