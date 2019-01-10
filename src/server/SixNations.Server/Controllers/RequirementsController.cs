using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixNations.Server.Data;
using SixNations.Server.Models;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Requirements/create
        [HttpGet("Create")]
        public async Task<IActionResult> CreateRequirement()
        {
            var requirement = new Requirement();
            await Task.CompletedTask;
            return Ok(requirement);
        }

        // GET: api/Requirements/5/edit
        [HttpGet("{id}/Edit")]
        public async Task<IActionResult> EditRequirement([FromRoute] int id)
        {
            var requirement = await GetRequirement(id);
            // TODO lock the record
            return Ok(requirement);
        }

        // GET: api/Requirements
        [HttpGet]
        public IEnumerable<Requirement> GetRequirement()
        {
            return _context.Requirements;
        }

        // GET: api/Requirements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequirement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requirement = await _context.Requirements.FindAsync(id);

            if (requirement == null)
            {
                return NotFound();
            }

            return Ok(requirement);
        }

        // PUT: api/Requirements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequirement([FromRoute] int id, [FromBody] Requirement requirement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requirement.RequirementID)
            {
                return BadRequest();
            }

            _context.Entry(requirement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequirementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requirements
        [HttpPost]
        public async Task<IActionResult> PostRequirement([FromBody] Requirement requirement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequirement", new { id = requirement.RequirementID }, requirement);
        }

        // DELETE: api/Requirements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requirement = await _context.Requirements.FindAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }

            _context.Requirements.Remove(requirement);
            await _context.SaveChangesAsync();

            return Ok(requirement);
        }

        private bool RequirementExists(int id)
        {
            return _context.Requirements.Any(e => e.RequirementID == id);
        }
    }
}