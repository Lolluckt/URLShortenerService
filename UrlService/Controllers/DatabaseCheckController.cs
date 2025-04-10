using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[Route("api/database-check")]
[ApiController]
public class DatabaseCheckController : ControllerBase
{
    private readonly IConfiguration _config;

    public DatabaseCheckController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult CheckDatabase()
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            return Ok("Database connection successful!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database connection failed: {ex.Message}");
        }
    }
}