using System.Data;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IDbConnection _connection;

        public AddressController(ILogger<AddressController> logger, IDbConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            string query = "SELECT * FROM Address";
            IEnumerable<Address> addresses = await _connection.QueryAsync<Address>(query);
            return Ok(addresses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAddressByID(int id)
        {
            string query = "SELECT * FROM Address WHERE AddressID = @AddressID";
            var address = await _connection.QueryAsync<Address>(query, new { AddressID = id });
            return Ok(address);
        }
    }
}
