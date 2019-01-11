using Microsoft.AspNetCore.Mvc;
using SixNations.Server.Data;
using SixNations.Server.Models;
using System.Collections.Generic;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementPriorityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequirementPriorityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RequirementPriority
        [HttpGet]
        public ResponseRootObject GetRequirementPriority()
        {
            var index = _context.RequirementPriority;
            var root = new ResponseRootObject(index);
            return root;
        }
    }
}