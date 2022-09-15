namespace GameShopAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int PaymentInfoId { get; set; }
        public PaymentInfo? PaymentInfo { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
