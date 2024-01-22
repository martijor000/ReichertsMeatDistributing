using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReichertsMeatDistributing.Shared;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly string _connection;

        public ProductsController(IConfiguration config)
        {
            _connection = config.GetConnectionString("Sqlite");
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductItem>>> Get()
        {
            using (var connection = new SqliteConnection(_connection))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM ProductItem";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var products = new List<ProductItem>();

                    while (await reader.ReadAsync())
                    {
                        var product = new ProductItem(
                            Id: reader.GetInt32(reader.GetOrdinal("Id")),
                            itemID: reader.GetString(reader.GetOrdinal("ItemID")),
                            stockingUM: reader.GetString(reader.GetOrdinal("StockingUM")),
                            itemDescription: reader.GetString(reader.GetOrdinal("ItemDescription")),
                            businessCategoryID: reader.GetInt32(reader.GetOrdinal("BusinessCategoryID"))
                        );

                        products.Add(product);
                    }

                    return products;
                }
            }
        }

        public async Task<ActionResult<List<BusinessCategory>>> GetCategories()
        {
            using (var connection = new SqliteConnection(_connection))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM BusinessCategory";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var businessCategory = new List<BusinessCategory>();

                    while (await reader.ReadAsync())
                    {
                        var category = new BusinessCategory(
                            id: reader.GetInt32(reader.GetOrdinal("Id")),
                            type: reader.GetString(reader.GetOrdinal("Type"))
                        );

                        businessCategory.Add(category);
                    }

                    return businessCategory;
                }
            }
        }
    }
}