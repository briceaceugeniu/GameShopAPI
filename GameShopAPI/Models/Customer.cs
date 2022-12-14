namespace GameShopAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Passkey { get; set; }
        public string Salt { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; } 
        public string? SessionToken { get; set; }
        public DateTime? SessionExpire { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
