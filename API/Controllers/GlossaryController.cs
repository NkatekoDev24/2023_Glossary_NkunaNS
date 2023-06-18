using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class GlossaryController : BaseApiController
    {
        private readonly DataContext _context;
        public GlossaryController(DataContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GlossaryTermDto>>> GetGlossaryTerms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var glossaryTerms = await _context.Glossaries
                .Where(g => g.UserId == userId) 
                .Select(g => new GlossaryTermDto
                {
                    Id = g.Id,
                    Date = g.Date,
                    Term = g.Term,
                    Definition = g.Definition
                })
                .ToListAsync();

            return glossaryTerms;
        }



        [HttpGet("pastmonth")]
        public async Task<ActionResult<IEnumerable<GlossaryTermDto>>> GetGlossaryTermsAddedOrUpdatedInPastMonth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pastMonthDate = DateTime.Now.AddDays(-30);

            var glossaryTerms = await _context.Glossaries
                .Where(g => g.UserId == userId && g.Date >= pastMonthDate)
                .Select(g => new GlossaryTermDto
                {
                    Id = g.Id,
                    Date = g.Date,
                    Term = g.Term,
                    Definition = g.Definition
                })
                .ToListAsync();

            return glossaryTerms;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<GlossaryTermDto>> GetGlossaryTermById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var glossaryTerm = await _context.Glossaries
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (glossaryTerm == null)
            {
                return NotFound("Glossary Term not found");
            }

            var glossaryTermDto = new GlossaryTermDto
            {
                Id = glossaryTerm.Id,
                Date = glossaryTerm.Date,
                Term = glossaryTerm.Term,
                Definition = glossaryTerm.Definition
            };

            return glossaryTermDto;
        }



        [HttpPost]
        public async Task<ActionResult<GlossaryTermDto>> CreateGlossaryTerm(GlossaryTermDto glossaryTermDto)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var glossaryTerm = new GlossaryTerm
                {
                    Date = glossaryTermDto.Date,
                    Term = glossaryTermDto.Term,
                    Definition = glossaryTermDto.Definition,
                    UserId = userId 
                };

                _context.Glossaries.Add(glossaryTerm);
                await _context.SaveChangesAsync();

                glossaryTermDto.Id = glossaryTerm.Id;
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var glossaryTerm = await _context.Glossaries.FindAsync(id);

            if (glossaryTerm == null)
            {
                return NotFound();
            }

            if (glossaryTerm.UserId != userId)
            {
                return Forbid(); // User does not have access to this glossary term
            }

            glossaryTerm.Date = glossaryTermDto.Date;
            glossaryTerm.Term = glossaryTermDto.Term;
            glossaryTerm.Definition = glossaryTermDto.Definition;

            _context.Entry(glossaryTerm).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlossaryTerm(int id)
        {
            // Get the currently authenticated user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var glossaryTerm = await _context.Glossaries.FindAsync(id);

            if (glossaryTerm == null)
            {
                return NotFound();
            }

            if (glossaryTerm.UserId != userId)
            {
                return Forbid(); // User does not have access to this glossary term
            }

            _context.Glossaries.Remove(glossaryTerm);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}