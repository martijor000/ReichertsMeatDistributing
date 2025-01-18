using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ReichertsMeatDistributing.Shared;

var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

// Retrieve the connection string
var connectionString = config.GetConnectionString("Default");

Console.WriteLine("Starting product import...");

var repository = new ProductRepository();
var products = repository.GetAllProducts();

using (var connection = new MySqlConnection(connectionString))
{
    connection.Open();

    foreach (var product in products)
    {
        // Insert each product into the database
        var command = new MySqlCommand(
            "INSERT INTO ProductsService (ItemId, StockingUm) VALUES (@code, @size)",
            connection);

        command.Parameters.AddWithValue("@code", product.ItemId);
        command.Parameters.AddWithValue("@size", product.StockingUm);

        command.ExecuteNonQuery();
    }

    Console.WriteLine("Product import completed.");
}