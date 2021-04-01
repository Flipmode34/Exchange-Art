using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        private string privateAddress { get; set; }
        public string publicAddress { get; set; }
        [Column("Wallet Balance" ,TypeName = "decimal(18,3)")]
        public decimal Balance { get; set; }
    }
}
