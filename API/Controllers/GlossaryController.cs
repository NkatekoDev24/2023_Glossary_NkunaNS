using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class GlossaryController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public GlossaryController(DataContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GlossaryTermDto>>> GetGlossaryTerms()
        {
            var glossaryTerms = await _context.Glossaries
                .Select(g => new GlossaryTermDto
                {
                    Id = g.Id,
                    Date = g.Date,
                    Term = g.Term,
                    Definition = g.Definition,
                    UserName = g.User.UserName // Assuming User is a property on the GlossaryTerm entity representing the associated user
                })
                .ToListAsync();

            return glossaryTerms;
        }




        [HttpGet("pastmonth")]
        public async Task<ActionResult<IEnumerable<GlossaryTermDto>>> GetGlossaryTermsAddedOrUpdatedInPastMonth()
        {
            var pastMonthDate = DateTime.Now.AddDays(-30);

            var glossaryTerms = await _context.Glossaries
                .Include(g => g.User) // Include the User navigation property
                .Where(g => g.Date >= pastMonthDate)
                .Select(g => new GlossaryTermDto
                {
                    Id = g.Id,
                    Date = g.Date,
                    Term = g.Term,
                    Definition = g.Definition,
                    UserName = g.User.UserName // Access the UserName property of the associated user
                })
                .ToListAsync();

            return glossaryTerms;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<GlossaryTermDto>> GetGlossaryTermById(int id)
        {
            var glossaryTerm = await _context.Glossaries
                .Include(g => g.User) // Include the User navigation property
                .FirstOrDefaultAsync(g => g.Id == id);

            if (glossaryTerm == null)
            {
                return NotFound("Glossary Term not found");
            }

            var glossaryTermDto = new GlossaryTermDto
            {
                Id = glossaryTerm.Id,
                Date = glossaryTerm.Date,
                Term = glossaryTerm.Term,
                Definition = glossaryTerm.Definition,
                UserName = glossaryTerm.User.UserName // Access the UserName property of the associated user
            };

            return glossaryTermDto;
        }

        [HttpPost]
        public async Task<ActionResult<GlossaryTermDto>> CreateGlossaryTerm(GlossaryTermDto glossaryTermDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == glossaryTermDto.UserName);

                if (user == null)
                {
                    return BadRequest("Invalid user"); // Return an appropriate error message if the user is not found
                }

                var glossaryTerm = new GlossaryTerm
                {
                    Date = glossaryTermDto.Date,
                    Term = glossaryTermDto.Term,
                    Definition = glossaryTermDto.Definition,
                    User = user
                };

                _context.Glossaries.Add(glossaryTerm);
                await _context.SaveChangesAsync();

                glossaryTermDto.Id = glossaryTerm.Id;
                glossaryTermDto.UserName = glossaryTerm.User.UserName;

                return CreatedAtAction(nameof(GetGlossaryTerms), new { id = glossaryTerm.Id }, glossaryTermDto);
            }

            return BadRequest(ModelState);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGlossaryTerm(int id, GlossaryTermDto glossaryTermDto)
        {
            if (id != glossaryTermDto.Id)
            {
                return BadRequest();
            }

            var glossaryTerm = await _context.Glossaries
                .Include(g => g.User) // Include the User navigation property
                .FirstOrDefaultAsync(g => g.Id == id);

            if (glossaryTerm == null)
            {
                return NotFound();
            }

            glossaryTerm.Date = glossaryTermDto.Date;
            glossaryTerm.Term = glossaryTermDto.Term;
            glossaryTerm.Definition = glossaryTermDto.Definition;

            // Assuming you don't update the associated user, no need to modify the User property

            _context.Entry(glossaryTerm).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlossaryTerm(int id)
        {
            var glossaryTerm = await _context.Glossaries.FindAsync(id);

            if (glossaryTerm == null)
            {
                return NotFound();
            }

            _context.Glossaries.Remove(glossaryTerm);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
