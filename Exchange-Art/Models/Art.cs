using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
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

        public double LeasePrice { get; set; } // Set LeasePrice in the LeaseController

        public bool Leased { get; set; }

        public byte[] ImageData { get; set; }
    }
}
