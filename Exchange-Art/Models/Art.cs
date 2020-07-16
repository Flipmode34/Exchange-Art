using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    public class Art
    {
        public int Id { get; set; }
        [Required]
        public string ImageTitle { get; set; }
        public string ImageDescription { get; set; } //TODO: add column in database table
        
        [Column("Owner")]
        public int UserId { get; set; } //TODO: add column in database table
        public User User { get; set; }

        public byte[] ImageData { get; set; }
    }
}
