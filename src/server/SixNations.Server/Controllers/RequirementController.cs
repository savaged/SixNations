using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixNations.Server.Data;
using SixNations.Server.Models;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public RequirementController(
            ApplicationDbContext context,
            ILogger<RequirementController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Requirement/create
        [HttpGet("Create")]
        public async Task<IActionResult> CreateRequirement()
        {
            var root = new ResponseRootObject(200, new Requirement());
            await Task.CompletedTask;
            return Ok(root);
        }

        // GET: api/Requirement/5/edit
        [HttpGet("{id}/Edit")]
        public async Task<IActionResult> EditRequirement([FromRoute] int id)
        {
            var requirement = await _context.Requirement.FindAsync(id);

            if (requirement == null)
            {
                return NotFound();
            }
            // Could lock the record here but EF Core handles concurrency just fine.

            var root = new ResponseRootObject(200, requirement);
            return Ok(root);
        }

        // GET: api/Requirement
        [HttpGet]
        public ResponseRootObject GetRequirement()
        {
            var index = _context.Requirement;
            var root = new ResponseRootObject(200, index);
            return root;
        }

        // GET: api/Requirement/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequirement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var requirement = await _context.Requirement.FindAsync(id);

            if (requirement == null)
            {
                return NotFound();
            }
            var root = new ResponseRootObject(200, requirement);
            return Ok(root);
        }

        // PUT: api/Requirement/5
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
            var root = new ResponseRootObject(200, requirement);
            return Ok(root);
        }

        // POST: api/Requirement
        [HttpPost]
        public async Task<IActionResult> PostRequirement([FromBody] Requirement requirement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Requirement.Add(requirement);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store new model in db");
                throw;
            }
            var root = new ResponseRootObject(201, requirement);
            return CreatedAtAction("GetRequirement", new { id = requirement.RequirementID }, root);
        }

        // DELETE: api/Requirement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requirement = await _context.Requirement.FindAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }

            _context.Requirement.Remove(requirement);
            await _context.SaveChangesAsync();

            var root = new ResponseRootObject(202);
            return AcceptedAtAction("GetRequirement", new { id = requirement.RequirementID }, root);
        }

        private bool RequirementExists(int id)
        {
            return _context.Requirement.Any(e => e.RequirementID == id);
        }
    }
}