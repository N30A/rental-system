using api.Models;
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
            throw new NotImplementedException();
        }

        public async Task<Result<AddressResponse>> GetAddressByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<MultipleAddressesResponse>> GetAddressesAsync(string sortColumn, string sortOrder)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AddressResponse>> UpdateAddressByIDAsync(int id, UpdateAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AddressResponse>> DeleteAddressAsync(int id)
        {
            throw new NotImplementedException();
        } 
    }
}
