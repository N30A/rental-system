using api.Models;
using api.Models.Dtos;

namespace api.Services.Interfaces
{
    public interface IAddressService
    {
        Task<Result<AddressResponse>> CreateAddressAsync(CreateAddressRequest request);

        Task<Result<AddressResponse>> GetAddressByIDAsync(int id);

        Task<Result<IEnumerable<AddressResponse>>> GetAddressesAsync(string sortColumn, string sortOrder);

        Task<Result<AddressResponse>> UpdateAddressByIDAsync(int id, UpdateAddressRequest request);

        Task<Result<AddressResponse>> DeleteAddressAsync(int id);
    }
}
