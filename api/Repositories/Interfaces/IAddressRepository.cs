﻿using api.Models;
using api.Models.Domain;
using api.Models.Dtos;

namespace api.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<Result<Address>> CreateAddressAsync(CreateAddressRequest request);

        Task<Result<Address>> GetAddressByIDAsync(int id);

        Task<Result<IEnumerable<Address>>> GetAddressesAsync(string sortColumn, string sortOrder);

        Task<Result<Address>> UpdateAddressByIDAsync(int id, UpdateAddressRequest request);

        Task<Result<Address>> DeleteAddressByIDAsync(int id);
    }
}
