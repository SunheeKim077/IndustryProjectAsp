using AspNetIndustryProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace AspNetIndustryProject.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        public List<Product> productList = new List<Product>();
        public Product product = new Product();

        // Reading connection string from secrets.json file using IConfiguration interface.
        private readonly IConfiguration configuration;

        public ProductController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [AllowAnonymous] // All users have an authorization.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string connectionString = this.configuration.GetConnectionString("defaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM Products";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                { 
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        { 
                            Product product = new Product();
                            product.Id = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.PricePerUnit = reader.GetDecimal(2);
                            product.Quantity = reader.GetString(3);

                            productList.Add(product);
                        }
                    }
                }
            }          
            return View(productList);
        }

        [Authorize (Roles ="Admin, Manager")] // Admin or Manager has an authorization.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            string connectionString = this.configuration.GetConnectionString("defaultConnection"); // secrets.json

            using (SqlConnection conn = new SqlConnection(connectionString)) 
            {
                await conn.OpenAsync();
                string query = "INSERT INTO Products (name, priceperunit, quantity) VALUES (@name, @priceperunit, @quantity)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product.Name);
                    cmd.Parameters.AddWithValue("@priceperunit", product.PricePerUnit);
                    cmd.Parameters.AddWithValue("@quantity", product.Quantity);
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        
                        await conn.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            await conn.CloseAsync();
                        }
                    }
                    return RedirectToAction("Index", "Product");
                }            
            }
        }

        [Authorize (Roles = "Admin")] // Only an Admin has an authorization.
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            // Reading ConnectionString from AppSettings.json file
            // using IConfiguration interface.
            string connectionString = this.configuration.GetConnectionString("defaultConnection");

            // Database connection.
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Insert parameter name instead of real user-input value.
                string query = "SELECT * FROM Products WHERE id = @id";

                // Create SqlCommand
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Use AddWithValue method to add a new parameter.
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            product.Id = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.PricePerUnit = reader.GetDecimal(2);
                            product.Quantity = reader.GetString(3);
                        }
                    }
                }
                return View(product);
            }           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            string connectionString = this.configuration.GetConnectionString("defaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "UPDATE Products SET name = @name, priceperunit = @priceperunit, quantity = @quantity WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.Id);
                    cmd.Parameters.AddWithValue("@name", product.Name);
                    cmd.Parameters.AddWithValue("@priceperunit", product.PricePerUnit);
                    cmd.Parameters.AddWithValue("@quantity", product.Quantity);

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        await conn.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            await conn.CloseAsync();
                        }
                    }
                    return RedirectToAction("Index", "Product");
                }
            } 
        }

        [Authorize(Policy = "RequireAdminRole")] // Using Policy property in Program.cs file.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string connectionString = this.configuration.GetConnectionString("defaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM Products WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            product.Id = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.PricePerUnit = reader.GetDecimal(2);
                            product.Quantity = reader.GetString(3);
                        }
                    }
                }
                return View(product);
            }
        }
        [Authorize (Policy = "RequireAdminRole")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            string connectionString = this.configuration.GetConnectionString("defaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "DELETE FROM Products WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return RedirectToAction("Index", "Product");
        }
    }
}
