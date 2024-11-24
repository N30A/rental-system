namespace api.Models.Dtos
{
    public class AddressResponse
    {
        public int AddressID { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public AddressResponse() { }
    }
}
