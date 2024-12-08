﻿using api.Models;
using api.Models.Domain;
using api.Models.Dtos;
using api.Repositories.Interfaces;
using api.Services.Interfaces;

namespace api.Services
{
    public class AddressService : IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _repository;

        public AddressService(ILogger<AddressService> logger, IAddressRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<Result<AddressResponse>> CreateAddressAsync(CreateAddressRequest request)
        {
            Result<Address> result = await _repository.CreateAddressAsync(request);
            if (!result.Success)
            {
                _logger.LogError($"Failed to create address: {result.Message}");
                return Result<AddressResponse>.FailureResult(result.Message, result.Error);
            }

            var addressResponse = new AddressResponse
            {
                AddressID = result.Data.AddressID,
                Street = result.Data.Street,
                PostalCode = result.Data.PostalCode,
                City = result.Data.City,
                Country = result.Data.Country,
            };

            return Result<AddressResponse>.SuccessResult(addressResponse);
        }

        public async Task<Result<AddressResponse>> GetAddressByIDAsync(int id)
        {
            Result<Address> result = await _repository.GetAddressByIDAsync(id);
            if (!result.Success)
            {
                _logger.LogError($"Failed to retrieve address with id {id}: {result.Message}");
                return Result<AddressResponse>.FailureResult(result.Message, result.Error);
            }

            var addressResponse = new AddressResponse
            {
                AddressID = result.Data.AddressID,
                Street = result.Data.Street,
                PostalCode = result.Data.PostalCode,
                City = result.Data.City,
                Country = result.Data.Country,
            };

            return Result<AddressResponse>.SuccessResult(addressResponse);
        }

        public async Task<Result<MultipleAddressesResponse>> GetAddressesAsync(string sortColumn, string sortOrder)
        {
            Result<IEnumerable<Address>> result = await _repository.GetAddressesAsync(sortColumn, sortOrder);
            if (!result.Success)
            {
                _logger.LogError($"Failed to retrieve addresses: {result.Message}");
                return Result<MultipleAddressesResponse>.FailureResult(result.Message, result.Error);
            }

            var addresses = new MultipleAddressesResponse
            {
                Data = result.Data.Select(a => new AddressResponse
                {
                    AddressID = a.AddressID,
                    Street = a.Street,
                    PostalCode = a.PostalCode,
                    City = a.City,
                    Country = a.Country,

                }).ToList(),
            };

            return Result<MultipleAddressesResponse>.SuccessResult(addresses);
        }

        public async Task<Result<AddressResponse>> UpdateAddressByIDAsync(int id, UpdateAddressRequest request)
        {
            Result<Address> result = await _repository.UpdateAddressByIDAsync(id, request);
            if (!result.Success)
            {
                _logger.LogError($"Failed to update address with id {id}: {result.Message}");
                return Result<AddressResponse>.FailureResult(result.Message, result.Error);
            }

            var addressResponse = new AddressResponse
            {
                AddressID = result.Data.AddressID,
                Street = result.Data.Street,
                PostalCode = result.Data.PostalCode,
                City = result.Data.City,
                Country = result.Data.Country,
            };

            return Result<AddressResponse>.SuccessResult(addressResponse);
        }

        public async Task<Result<AddressResponse>> DeleteAddressByIDAsync(int id)
        {
            Result<Address> result = await _repository.DeleteAddressByIDAsync(id);
            if (!result.Success)
            {
                _logger.LogError($"Failed to delete address with id {id}: {result.Message}");
                return Result<AddressResponse>.FailureResult(result.Message, result.Error);
            }

            var addressResponse = new AddressResponse
            {
                AddressID = result.Data.AddressID,
                Street = result.Data.Street,
                PostalCode = result.Data.PostalCode,
                City = result.Data.City,
                Country = result.Data.Country,
            };

            return Result<AddressResponse>.SuccessResult(addressResponse);
        }
    }
}
