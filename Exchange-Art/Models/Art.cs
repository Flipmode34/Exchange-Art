using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    // Database table dbo.Art

    public class Art
    {
        public int Id { get; set; }
        
        [Required]
        public string ImageTitle { get; set; }
        public string ImageDescription { get; set; }
        
        [ForeignKey("ApplicationUser"), Column("OwnerId")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string OwnerName { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal LeasePrice { get; set; } // change to INT (02092020)

        public bool Leased { get; set; }

        public byte[] ImageData { get; set; }
    }
}