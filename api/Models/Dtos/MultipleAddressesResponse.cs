namespace api.Models.Dtos
{
    public class MultipleAddressesResponse
    {
        public int Count => Data.Count();
        public IEnumerable<AddressResponse> Data { get; set; } = new List<AddressResponse>();
        
        public MultipleAddressesResponse() { }
    }
}
