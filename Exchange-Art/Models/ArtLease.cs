using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    public class ArtLease
    {
        public int Id {get; set;}

        [ForeignKey("Art"), Column("ArtPiece")]
        public string ArtId { get; set; }
        public Art ArtPiece { get; set; }

        [ForeignKey("ApplicationUser"), Column("ArtLeaser")]
        public string LeaserId { get; set; }
        public ApplicationUser LeaseUser { get; set; }

        public int LeasePeriod { get; set; }

        public int LeaseDaysleft { get; set; }

        [Column("LeaseAmount")]
        public double CryptoAmount { get; set; }

        [ForeignKey("ApplicationUser"), Column("ArtOwner")]
        public string OwnerId { get; set; }
        public ApplicationUser OwnerUser { get; set; }

    }
}
