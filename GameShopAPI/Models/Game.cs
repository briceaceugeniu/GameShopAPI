namespace GameShopAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PublisherId{ get; set; }
        public Publisher? Publisher { get; set; }
        public int DeveloperId { get; set; }
        public Developer? Developer { get; set; }
        public decimal Price { get; set; }
        public string? ImgPath { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Description { get; set; }
        public int GameCategoryId { get; set; }
        public GameCategory? GameCategory { get; set; }
    }
}