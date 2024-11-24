namespace api.Models.Dto
{
    public class UpdateAddressRequest
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public UpdateAddressRequest() { }
    }
}
