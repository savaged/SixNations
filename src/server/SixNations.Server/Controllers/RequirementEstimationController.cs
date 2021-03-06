﻿using Microsoft.AspNetCore.Mvc;
using SixNations.Server.Data;
using SixNations.Server.Models;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementEstimationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequirementEstimationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RequirementEstimation
        [HttpGet]
        public ResponseRootObject GetRequirementEstimation()
        {
            var index = _context.RequirementEstimation;
            var root = new ResponseRootObject(200, index);
            return root;
        }
    }
}