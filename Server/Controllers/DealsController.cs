using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReichertsMeatDistributing.Shared;
using Dapper;
using System.Data;
using System.Data.SQLite;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public string connectionId = "Default";
        public string sqlCommand = "";
        IEnumerable<Deal>? _deals;

        public DealsController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/deals
        [HttpGet]
        public async Task<ActionResult<List<Deal>>> Get()
        {
            using IDbConnection conn = new SQLiteConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "SELECT * FROM Deal";
                var result = await conn.QueryAsync<Deal>(sqlCommand);
                return Ok(result.ToList());
            }
        }

        // GET api/deals/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Deal>> GetById(int id)
        {
            using IDbConnection conn = new SQLiteConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "SELECT * FROM Deal WHERE Id=@Id";
                var result = await conn.QuerySingleOrDefaultAsync<Deal>(sqlCommand, new { Id = id });

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
        }

        // POST api/deals
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Deal deal)
        {
            using IDbConnection conn = new SQLiteConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "INSERT INTO Deal (Name, Description, Price) VALUES (@Name, @Description, @Price); SELECT last_insert_rowid()";
                var newId = await conn.ExecuteScalarAsync<int>(sqlCommand, deal);

                return CreatedAtAction(nameof(GetById), new { id = newId }, deal);
            }
        }

        // PUT api/deals/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Deal deal)
        {
            using IDbConnection conn = new SQLiteConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "UPDATE Deal SET Name=@Name, Description=@Description, Price=@Price WHERE Id=@Id";
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
            using IDbConnection conn = new SQLiteConnection(_config.GetConnectionString(connectionId));
            {
                string sqlCommand = "DELETE FROM Deal WHERE Id=@Id";
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
