using System;
using System.Collections.Generic;

namespace Exchange_Art.Models
{
    public class Block
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
