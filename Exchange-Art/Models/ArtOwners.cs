using System.Collections.Generic;

namespace Exchange_Art.Models
{
    // This is a ViewModel class for the GET: Update Action in the UsersController
    // This is not a Database table
    public class ArtOwners
    {
        public ApplicationUser ArtOwner { get; set; }
        public IEnumerable<Art> ArtPieces { get; set; }
    }
}
