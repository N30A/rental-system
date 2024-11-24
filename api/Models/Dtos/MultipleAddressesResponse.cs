namespace api.Models.Dtos
{
    public class MultipleAddressesResponse
    {   
        public IEnumerable<AddressResponse> Data { get; set; } = new List<AddressResponse>();
        public int Count => Data.Count();
        
        public MultipleAddressesResponse() { }
    }
}
