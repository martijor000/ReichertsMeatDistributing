using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReichertsMeatDistributing.Shared;
using System.Collections.Generic;
using System.Data;
using Dapper;
using ReichertsMeatDistributing.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductItem>>> Get()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }
    }
}
