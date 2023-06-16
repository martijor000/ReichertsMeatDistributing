using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using ReichertsClassLib;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public string connectionId = "Default";
        public string sqlCommand = "";
        IEnumerable<WeeklyDeal>? _deals;

        public DealsController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/deals
        [HttpGet]
        public async Task<ActionResult<List<WeeklyDeal>>> Get()
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "SELECT * FROM WeeklyDeal";
                var result = await conn.QueryAsync<WeeklyDeal>(sqlCommand);
                return Ok(result.ToList());
            }
        }

        // GET api/deals/1
        [HttpGet("{id}")]
        public async Task<ActionResult<WeeklyDeal>> GetById(int id)
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "SELECT * FROM WeeklyDeal WHERE Id=@Id";
                var result = await conn.QuerySingleOrDefaultAsync<WeeklyDeal>(sqlCommand, new { Id = id });

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
        }

        // POST api/deals
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WeeklyDeal deal)
        {
                using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
                {
                    string sqlCommand = "INSERT INTO WeeklyDeal (Name, Description, Price) VALUES (@Name, @Description, @Price); SELECT LAST_INSERT_ID()";
                    var newId = await conn.ExecuteScalarAsync<int>(sqlCommand, deal);

                    deal.Id = newId; // Set the ID of the deal object to the newly generated ID

                    return CreatedAtAction(nameof(GetById), new { id = newId }, deal);
                }
        }

        // PUT api/deals/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] WeeklyDeal deal)
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "UPDATE WeeklyDeal SET Name=@Name, Description=@Description, Price=@Price WHERE Id=@Id";
                var rowsUpdated = await conn.ExecuteAsync(sqlCommand, new { Name = deal.Name, Description = deal.Description, Price = deal.Price, Id = id });

                if (rowsUpdated == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        // DELETE api/deals/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "DELETE FROM WeeklyDeal WHERE Id=@Id";
                var rowsDeleted = await conn.ExecuteAsync(sqlCommand, new { Id = id });

                if (rowsDeleted == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }
    }
}
