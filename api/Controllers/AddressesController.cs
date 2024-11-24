using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FluentValidation;
using FluentValidation.Results;
using api.Models;
using api.Models.Domain;
using api.Models.Dtos;
using api.Validators;
using api.Services.Interfaces;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddressesController : ControllerBase
    {   
        private readonly ILogger<AddressesController> _logger;
        private readonly IAddressService _service;
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
            IAddressService service,
            AddressCreateValidator createValidator,
            AddressUpdateValidator updateValidator
        )
        {   
            _logger = logger;
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<MultipleAddressesResponse>> GetAddresses([FromQuery] string sort = "addressid", [FromQuery] string order = "asc")
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

            Result<MultipleAddressesResponse> result = await _service.GetAddressesAsync(sortColumn, sortOrder);
            
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = "An unexpected error occurred while fetching addresses.",
                    Status = StatusCodes.Status500InternalServerError,
                });
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAddressByID(int id)
        {   
            Result<AddressResponse> result = await _service.GetAddressByIDAsync(id);
            if (!result.Success)
            {   
                switch (result.Error)
                {
                    case ErrorCodes.NotFound:
                        return NotFound(new ProblemDetails
                        {
                            Title = "Address not found",
                            Detail = result.Message,
                            Status = StatusCodes.Status404NotFound
                        });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                        {
                            Title = "Internal error",
                            Detail = result.Message,
                            Status = StatusCodes.Status500InternalServerError
                        });
                }
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress([FromBody] CreateAddressRequest request)
        {   
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ForEach(error =>
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                });
                return ValidationProblem(ModelState);
            }

            Result<AddressResponse> result = await _service.CreateAddressAsync(request);
            if (!result.Success)
            {
                switch (result.Error)
                {
                    case ErrorCodes.UniqueConflict:
                        return StatusCode(StatusCodes.Status409Conflict, new ProblemDetails
                        {
                            Title = "Already exists",
                            Detail = result.Message,
                            Status = StatusCodes.Status409Conflict
                        });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                        {
                            Title = "Internal error",
                            Detail = result.Message,
                            Status = StatusCodes.Status500InternalServerError
                        });
                }
            }

            return CreatedAtAction(nameof(GetAddressByID), new { id = result.Data?.AddressID }, result?.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Address>> UpdateAddress(int id, [FromBody] UpdateAddressRequest body)
        {
            var validationResults = _updateValidator.Validate(body);
            if (!validationResults.IsValid)
            {
                validationResults.Errors.ForEach(error =>
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                });
                return ValidationProblem(ModelState);
            }

            Result<AddressResponse> result = await _service.UpdateAddressByIDAsync(id, body);
            if (!result.Success)
            {   
                switch (result.Error)
                {
                    case ErrorCodes.NotFound:
                        return NotFound(new ProblemDetails
                        {
                            Title = "Address not found",
                            Detail = result.Message,
                            Status = StatusCodes.Status404NotFound
                        });

                    case ErrorCodes.UniqueConflict:
                        return StatusCode(StatusCodes.Status409Conflict, new ProblemDetails
                        {
                            Title = "Already exists",
                            Detail = result.Message,
                            Status = StatusCodes.Status409Conflict
                        });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                        {
                            Title = "Internal error",
                            Detail = result.Message,
                            Status = StatusCodes.Status500InternalServerError
                        });
                }
            }

            return CreatedAtAction(nameof(GetAddressByID), new { id = result.Data?.AddressID }, result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAddressByID(int id)
        {
            Result<AddressResponse> result = await _service.DeleteAddressByIDAsync(id);
            if (!result.Success)
            {   
                switch (result.Error)
                {
                    case ErrorCodes.NotFound:
                        return NotFound(new ProblemDetails
                        {
                            Title = "Address not found",
                            Detail = result.Message,
                            Status = StatusCodes.Status404NotFound
                        });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                        {
                            Title = "Internal error",
                            Detail = result.Message,
                            Status = StatusCodes.Status500InternalServerError
                        });
                }
            }

            return NoContent();
        }
    }
}
