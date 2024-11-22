namespace api.Dto
{
    public class AddressCreate
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public AddressCreate() { }
    }
}
