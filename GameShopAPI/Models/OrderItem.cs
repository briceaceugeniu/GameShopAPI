﻿namespace GameShopAPI.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public decimal Price { get; set; }
    }
}
