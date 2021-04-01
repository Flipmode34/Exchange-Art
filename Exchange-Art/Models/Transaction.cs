using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange_Art.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        [Column("LeaseAmount", TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }
        public bool RewardTrx { get; set; }
        public bool Processed { get; set; }
    }
}
