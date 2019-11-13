using Microsoft.AspNetCore.Mvc;
using SixNations.Server.Data;
using SixNations.Server.Models;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseRootRequirementStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResponseRootRequirementStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RequirementStatus
        [HttpGet]
        public ResponseRootObject GetRequirementStatus()
        {
            var index = _context.RequirementStatus;
            var root = new ResponseRootObject(200, index);
            return root;
        }
    }
}