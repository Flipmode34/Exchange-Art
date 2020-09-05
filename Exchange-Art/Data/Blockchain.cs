using System;
using System.Collections.Generic;

namespace Exchange_Art.Data
{
    public class Blockchain
    {
        public IList<Block> Chain { get; set; }
        public int Difficulty { set; get; } = 2;
        public int Reward = 1; // 1 cryptocurrency

        IList<Transaction> PendingTransactions = new List<Transaction>();

        // Constructor
        public Blockchain()
        {
            InitializeChain();
            AddGenesisBlock();
        }

        // Create a new Blockchain
        public void InitializeChain()
        {
            //TODO: Read Block records from the SQL table (Blocks) into a List
            Chain = new List<Block>();
        }

        // Run only once, during Blockchain creation
        public Block CreateGenesisBlock()
        {
            Block block = new Block(DateTime.Now, null, PendingTransactions);
            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        // Add genesis block to the newly created Blockchain
        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }

        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }

        public void ProcessPendingTransactions(string minerAddress)
        {
            Block block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);
            AddBlock(block);

            PendingTransactions = new List<Transaction>();
            CreateTransaction(new Transaction(null, minerAddress, Reward));
        }

        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();
            block.BlockIndex = latestBlock.BlockIndex + 1;
            block.PreviousHash = latestBlock.Hash;
            block.Mine(this.Difficulty); // Increase Nonce if necessary
            Chain.Add(block);
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
