using Dapper;
using api.Models;
using api.Dto;
using System.Data;
using Microsoft.Data.SqlClient;
using api.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace api.Repositories
{
    public class AddressRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<AddressRepository> _logger;

        public AddressRepository(IDbConnection connection, ILogger<AddressRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(string sortColumn, string sortOrder)
        {
            string query = @"
                DECLARE @Query NVARCHAR(MAX);
                SET @Query = N'
                    SELECT AddressID, Street, PostalCode, City, Country
                    FROM Address
                    ORDER BY ' + QUOTENAME(@SortColumn) + ' ' + @SortOrder;

                EXEC sp_executesql @Query;
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@SortColumn", sortColumn);
            parameters.Add("@SortOrder", sortOrder);

            try
            {
                return await _connection.QueryAsync<Address>(query, parameters);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when retrieving addresses");
                throw;
            }
        }

        public async Task<Address?> GetAddressByIDAsync(int id)
        {
            string query = @"
                SELECT AddressID, Street, PostalCode, City, Country
                FROM Address
                WHERE AddressID = @AddressID;
            ";

            try
            {
                return await _connection.QuerySingleOrDefaultAsync<Address>(query, new { AddressID = id });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when retrieving the specified address");
                throw;
            }
        }

        public async Task<Address> CreateAddressAsync(AddressCreate addressCreate)
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
                return await _connection.QuerySingleAsync<Address>(insertQuery, addressCreate);
            }
            catch (SqlException ex) when (ex.Number == UniqueConstraintException.Number)
            {
                _logger.LogWarning($"{ex.Message} ErrorNumber: {ex.Number}");
                throw new UniqueConstraintException("The address already exists.", ex);
            }
            catch (SqlException ex)
            {   
                _logger.LogError(ex, "Unexpected error when creating an address");
                throw;
            }
        }

        public async Task<Address?> UpdateAddressByIDAsync(int id, AddressUpdate addressUpdate)
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

            var parameters = new DynamicParameters();
            parameters.Add("@AddressID", id);
            parameters.Add("@Street", addressUpdate.Street);
            parameters.Add("@PostalCode", addressUpdate.PostalCode);
            parameters.Add("@City", addressUpdate.City);
            parameters.Add("@Country", addressUpdate.Country);

            try
            {   
                return await _connection.QuerySingleOrDefaultAsync<Address>(updateQuery, parameters);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when updating the address");
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            string deleteQuery = @"
                DELETE FROM Address
                WHERE AddressID = @AddressID;
            ";

            try
            {
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { AddressID = id });
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unexpected error when deleting the address");
                throw;
            }
        }
    }
}
