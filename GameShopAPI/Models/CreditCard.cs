namespace GameShopAPI.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string Owner { get; set; }
        public string Number { get; set; }
        public int CVC { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}