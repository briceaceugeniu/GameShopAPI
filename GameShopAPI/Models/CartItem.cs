using System.ComponentModel.DataAnnotations.Schema;

namespace GameShopAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
