using API.Entitities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GlossaryTerm> Glossaries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the relationship between GlossaryTerm and AppUser
            builder.Entity<GlossaryTerm>()
                .HasOne(gt => gt.User)
                .WithMany(u => u.GlossaryTerms)
                .HasForeignKey(gt => gt.UserName)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}