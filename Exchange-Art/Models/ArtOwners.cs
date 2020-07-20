using System.Collections.Generic;

namespace Exchange_Art.Models
{
    // This is a ViewModel class for the GET: Upload method
    public class ArtOwners
    {
        public ApplicationUser ArtOwner { get; set; }
        public IEnumerable<Art> ArtPieces { get; set; }
    }
}
