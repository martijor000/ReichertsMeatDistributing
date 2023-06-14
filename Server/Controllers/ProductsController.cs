//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ReichertsMeatDistributing.Shared;
//using MySql.Data.MySqlClient;
//using System.Data;
//using Microsoft.Data.SqlClient;
//using Dapper;

//namespace ReichertsMeatDistributing.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductsController : ControllerBase
//    {
//        private readonly IConfiguration _config;
//        public string connectionId = "Default";
//        public string sqlCommand = "";
//        IEnumerable<Product>? _products;

//        public ProductsController(IConfiguration config)
//        {
//            _config = config;
//        }

//        // GET api/deals
//        [HttpGet]
//        public async Task<ActionResult<List<Product>>> Get()
//        {
//            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionId));
//            {
//                string sqlCommand = "SELECT * FROM Product";
//                var result = await conn.QueryAsync<Product>(sqlCommand);
//                return Ok(result.ToList());
//            }
//        }
//    }
//}
