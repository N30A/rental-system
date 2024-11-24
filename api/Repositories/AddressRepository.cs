using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using api.Models;
using api.Models.Dtos;
using api.Models.Domain;
using api.Repositories.Interfaces;

namespace api.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<AddressRepository> _logger;

        public AddressRepository(IDbConnection connection, ILogger<AddressRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Address>>> GetAddressesAsync(string sortColumn, string sortOrder)
        {
            string query = @"
                DECLARE @Query NVARCHAR(MAX);
                SET @Query = N'
                    SELECT AddressID, Street, PostalCode, City, Country
                    FROM Address
                    ORDER BY ' + QUOTENAME(@SortColumn) + ' ' + @SortOrder;

                EXEC sp_executesql @Query;
            ";

            try
            {
                IEnumerable<Address> addresses = await _connection.QueryAsync<Address>(query, new { SortColumn = sortColumn, SortOrder = sortOrder });
                return Result<IEnumerable<Address>>.SuccessResult(addresses);
            }
            catch (SqlException ex)
            {   
                _logger.LogError(ex, "Unexpected database error occurred while retrieving addresses.");
                return Result<IEnumerable<Address>>.FailureResult("An error occurred while retrieving addresses.");
            }
        }

        public async Task<Result<Address>> GetAddressByIDAsync(int id)
        {
            string query = @"
                SELECT AddressID, Street, PostalCode, City, Country
                FROM Address
                WHERE AddressID = @AddressID;
            ";

            try
            {   
                Address? address = await _connection.QuerySingleOrDefaultAsync<Address>(query, new { AddressID = id });
                if (address == null)
                {
                    return Result<Address>.FailureResult($"The address with id {id} was not found.", ErrorCodes.NotFound);
                }

                return Result<Address>.SuccessResult(address);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving the specified address.");
                return Result<Address>.FailureResult("An error occured while retrieving the address.", ErrorCodes.UnexpectedError);
            }
        }

        public async Task<Result<Address>> CreateAddressAsync(CreateAddressRequest addressCreate)
        {
            string insertQuery = @"
                INSERT INTO Address (Street, PostalCode, City, Country)
                OUTPUT 
		            INSERTED.AddressID,
		            INSERTED.Street,
		            INSERTED.PostalCode,
		            INSERTED.City,
		            INSERTED.Country
                VALUES (@Street, @PostalCode, @City, @Country);
            ";

            try
            {   
                Address address = await _connection.QuerySingleAsync<Address>(insertQuery, addressCreate);
                return Result<Address>.SuccessResult(address);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorNumbers.UniqueConstraintViolation)
            {
                _logger.LogWarning($"{ex.Message} ErrorNumber: {ex.Number}");
                return Result<Address>.FailureResult("This address already exists, duplicates not allowed.", ErrorCodes.UniqueConflict);
            }
            catch (SqlException ex)
            {   
                _logger.LogError(ex, "Unexpected error when creating an address");
                return Result<Address>.FailureResult("An error occured while creating the address.", ErrorCodes.UnexpectedError);
            }
        }

        public async Task<Result<Address>> UpdateAddressByIDAsync(int id, UpdateAddressRequest addressUpdate)
        {
            string updateQuery = @"
                UPDATE Address
                SET 
                    Street = CASE 
                                WHEN Street <> @Street THEN @Street 
                                ELSE Street 
                                END,
                    PostalCode = CASE 
                                    WHEN PostalCode <> @PostalCode THEN @PostalCode 
                                    ELSE PostalCode 
                                    END,
                    City = CASE 
                                WHEN City <> @City THEN @City 
                                ELSE City 
                            END,
                    Country = CASE 
                                WHEN Country <> @Country THEN @Country 
                                ELSE Country 
                                END
                OUTPUT 
                    INSERTED.AddressID,
                    INSERTED.Street,
                    INSERTED.PostalCode,
                    INSERTED.City,
                    INSERTED.Country
                WHERE AddressID = @AddressID;
            ";

            try
            {   
                Address? address = await _connection.QuerySingleOrDefaultAsync<Address>(updateQuery, new
                {
                    AddressID = id,
                    addressUpdate.Street,
                    addressUpdate.PostalCode,
                    addressUpdate.City,
                    addressUpdate.Country
                });

                if (address == null)
                {
                    _logger.LogWarning("Tried to update an address but it was not found.");
                    return Result<Address>.FailureResult($"The address with id {id} was not found.", ErrorCodes.NotFound);
                }
                
                return Result<Address>.SuccessResult(address);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when updating the address");
                return Result<Address>.FailureResult("An error occurred error when updating the address", ErrorCodes.UnexpectedError);
            }
        }

        public async Task<Result<Address>> DeleteAddressByIDAsync(int id)
        {
            string deleteQuery = @"
                DELETE FROM Address
                WHERE AddressID = @AddressID;
            ";

            try
            {
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { AddressID = id });
                if (rowsAffected <= 0)
                {
                    return Result<Address>.FailureResult($"The address with id {id} was not found.", ErrorCodes.NotFound);
                }

                return Result<Address>.SuccessResult();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when deleting the address");
                return Result<Address>.FailureResult("An error occurred error when deleting the address", ErrorCodes.UnexpectedError);
            }
        }
    }
}
