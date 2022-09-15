namespace GameShopAPI.Models
{
    public class PaymentInfo
    {
        public int Id { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }
        public int CreditCardId { get; set; }
        public CreditCard? CreditCard { get; set; }
    }
}