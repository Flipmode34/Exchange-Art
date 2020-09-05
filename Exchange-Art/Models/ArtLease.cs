using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    // Database table dbo.ArtLease

    public class ArtLease
    {

        public int Id {get; set;}

        [ForeignKey("Art"), Column("ArtPiece")]
        public int ArtId { get; set; }

        public byte[] ArtPieceImageData { get; set; }

        public string ArtDescription { get; set; }

        [ForeignKey("ApplicationUser"), Column("ArtLeaser")]
        public string LeaserId { get; set; }

        public string LeaserName { get; set; }

        public int LeasePeriodInMonths { get; set; }

        public string DateLeaseStarted { get; set; }

        public string DateLeaseEnds { get; set; }

        [Column("LeaseAmount", TypeName = "decimal(18,3)")]
        public decimal CryptoAmount { get; set; } // Change to INT (02092020)

        [ForeignKey("ApplicationUser"), Column("ArtOwner")]
        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

    }
}