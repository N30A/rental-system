using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using api.Repositories;
using api.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
using FluentValidation;
using System.Reflection.Metadata;
using api.Models.Dto;
using api.Models.Domain;
using api.Validators;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddressesController : ControllerBase
    {   
        private readonly ILogger<AddressesController> _logger;
        private readonly AddressRepository _repository;
        private readonly AddressCreateValidator _createValidator;
        private readonly AddressUpdateValidator _updateValidator;

        private static readonly Dictionary<string, string> SortOptions = new()
        {
            { "addressid", "AddressID" },
            { "street", "Street" },
            { "postalcode", "PostalCode" },
            { "city", "City" },
            { "country", "Country" }
        };

        private static readonly Dictionary<string, string> OrderOptions = new()
        {
            { "asc", "ASC" },
            { "desc", "DESC" }
        };

        public AddressesController(
            ILogger<AddressesController> logger,
            AddressRepository repository,
            AddressCreateValidator createValidator,
            AddressUpdateValidator updateValidator
        )
        {   
            _logger = logger;
            _repository = repository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAddresses([FromQuery] string sort = "addressid", [FromQuery] string order = "asc")
        {
            if (string.IsNullOrWhiteSpace(sort) || string.IsNullOrWhiteSpace(order))
            {
                ModelState.AddModelError("sort", "The supplied value must be a valid attribute.");
                ModelState.AddModelError("order", "The supplied value must be asc or desc.");
                return ValidationProblem(ModelState);
            }

            sort = sort.ToLower();
            order = order.ToLower();

            if(!SortOptions.TryGetValue(sort, out string? sortColumn))
            {
                ModelState.AddModelError("sort", "The supplied value must be a valid attribute.");
                return ValidationProblem(ModelState);
            }

            if (!OrderOptions.TryGetValue(order, out string? sortOrder))
            {
                ModelState.AddModelError("order", "The supplied value must be asc or desc.");
                return ValidationProblem(ModelState);
            }

            try
            {
                IEnumerable<Address> addresses = await _repository.GetAddressesAsync(sortColumn, sortOrder);
                return Ok(addresses);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while fetching addresses.",
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAddressByID(int id)
        {   
            try
            {
                Address? address = await _repository.GetAddressByIDAsync(id);
                if (address == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Address not found",
                        Detail = "The specified address was not found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(address);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while fetching the specified address.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress([FromBody] AddressCreate body)
        {   
            var results = _createValidator.Validate(body);
            if (!results.IsValid)
            {
                results.Errors.ForEach(error =>
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                });
                return ValidationProblem(ModelState);
            }

            try
            {
                Address address = await _repository.CreateAddressAsync(body);
                return CreatedAtAction(nameof(GetAddressByID), new { id = address.AddressID }, address);
            }
            catch (UniqueConstraintException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ProblemDetails
                {
                    Title = "Already exists",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (SqlException)
            {   
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while creating the specified address.",
                    Status = StatusCodes.Status500InternalServerError
                });
            } 
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Address>> UpdateAddress(int id, [FromBody] AddressUpdate body)
        {
            var results = _updateValidator.Validate(body);
            if (!results.IsValid)
            {
                results.Errors.ForEach(error =>
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                });
                return ValidationProblem(ModelState);
            }

            try
            {
                Address? address = await _repository.UpdateAddressByIDAsync(id, body);
                if (address == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Address not found",
                        Detail = "The specified address was not found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return CreatedAtAction(nameof(GetAddressByID), new { id = address.AddressID }, address);
            }
            catch (UniqueConstraintException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ProblemDetails
                {
                    Title = "Already exists",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while creating the specified address.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAddressByID(int id)
        {
            try
            {
                bool status = await _repository.DeleteAddressAsync(id);
                if (!status)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Address not found",
                        Detail = "The specified address was not found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return NoContent();
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while fetching the specified address.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
