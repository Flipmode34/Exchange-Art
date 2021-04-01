using Exchange_Art.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Exchange_Art.Data
{
    public class FlipChain : IFlipChain
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        static readonly Random random = new Random();
        private decimal startingBalance = 20;
        private IEnumerable<Block> Chain { get; set; }
        private int Difficulty { get; set; } = 1; // Difficulty is static for now
        private decimal Reward { get; set; } = 1; // 1 FlipCoin

        // Constructor
        public FlipChain(ApplicationDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        private void CreateChain()
        {
            try
            {
                Chain = _context.Blocks.ToList<Block>();
                Console.WriteLine("There are existing Blocks in the database, GenesisBlock is NOT created.");
                Console.WriteLine($"Timestamp of GenesisBlock: {_context.Blocks.LastOrDefault<Block>().TimeStamp}\n");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("No block is present in the database, GenesisBlock will be created and added to database.");
                AddGenesisBlock(); // Only adds GenesisBlock if it doesn't exist yet
                throw;
            }

            //// Read blocks from SQL table and puts them in a List
            //List<Block> blocks = (
            //from b in _context.Blocks
            //orderby b.Id
            //select b).ToList();

            //// Creates the actual List<Block> of blocks, stored in the database table "Blocks"
            //foreach (Block block in blocks)
            //{
            //    Chain.Add(block);
            //}

            //if (Chain.Count <= 0)
            //{
            //    Console.WriteLine("No block is present in the database, GenesisBlock will be created and added to database.");
            //    AddGenesisBlock(); // Only adds GenesisBlock if it doesn't exist yet
            //}
            //else
            //{
            //    Console.WriteLine("There are existing Blocks in the database, GenesisBlock is NOT created.");
            //    Console.WriteLine($"Timestamp of GenesisBlock: {Chain[0].TimeStamp}\n");
            //}
        }

        // Uses SHA1 since this is not a Production-like CryptoCurrency, otherwise SHA256
        private string CalculateHash(DateTime dateTime, string PreviousHash, List<Transaction> transactions, int nonce)
        {
            SHA1 sha1 = SHA1.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(
                $"{dateTime}-{PreviousHash ?? ""}-{JsonConvert.SerializeObject(transactions)}-{nonce}");
            byte[] outputBytes = sha1.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        private Block CreateGenesisBlock()
        {
            Block GenesisBlock = new Block();

            GenesisBlock.TimeStamp = DateTime.Now;
            GenesisBlock.PreviousHash = null;
            GenesisBlock.Nonce = 0;
            GenesisBlock.Transactions = new List<Transaction>();
            GenesisBlock.Hash = CalculateHash(
                GenesisBlock.TimeStamp,
                null,
                GenesisBlock.Transactions,
                GenesisBlock.Nonce);

            return GenesisBlock;
        }

        public void AddGenesisBlock()
        {
            // Check if GenesisBlock already exists
            var Block = (
                        from r in _context.Blocks
                        select r).FirstOrDefault();

            if (Block == null)
            {
                // Create GenesisBlock
                Block GenesisBlock = CreateGenesisBlock();
                // Save GenesisBlock in database
                StoreBlockInDatabase(GenesisBlock);

                _notyf.Success("Genesis Block added to Database!");
            }
            else
            {
                _notyf.Error("Genesis Block already exists!");
            }
        }

        // Creates random Wallet address
        private static string RandomStringAddress(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Wallet CreateAndStoreWalletInDatabase(string username, string emailaddress)
        {
            Wallet wallet = new Wallet();
            wallet.Username = username;
            wallet.Email = emailaddress;

            string randomAddress = RandomStringAddress(12);

            // Check if randomly generated address is already used (What will be the odds eh?)
            while (GeneratedWalletAddressIsTaken(randomAddress))
                randomAddress = RandomStringAddress(12);

            wallet.publicAddress = randomAddress;
            wallet.Balance = startingBalance;

            _context.Wallets.Add(wallet);
            _context.SaveChanges(); // Db is local, no need for async

            return wallet;
        }

        public void DeleteWalletFromDatabase(int Id)
        {
            var wallet = _context.Wallets.Find(Id);
            _context.Wallets.Remove(wallet);
            _context.SaveChanges(); // Db is local, no need for async
        }

        // Checks if the randomly generated wallet address is unique yes or no.
        // Returns TRUE if it is unique, otherwise FALSE.
        private bool GeneratedWalletAddressIsTaken(string GeneratedWalletAddress)
        {
            List<string> walletList = new List<string>();
            foreach (Wallet walletItem in GetAllWallets())
            {
                walletList.Add(walletItem.publicAddress);
            }
            return walletList.Any(s => s.Contains(GeneratedWalletAddress)) ? true : false;
        }

        private bool GeneratedTransactionIdIsUnique(int generatedId)
        {
            List<int> Idlist = GetAllTransactionIds();

            bool exists = Idlist.IndexOf(generatedId) != -1;
            if (!exists)
                return true;
            else
                return false;
        }

        public string GetWalletAddress(string username)
        {
            var address = (
                from w in _context.Wallets
                where w.Username == username
                select w.publicAddress).FirstOrDefault();

            return address;
        }

        private string GetRandomWalletAddress()
        {
            try
            {
                var addresses = (
                        from a in _context.Wallets
                        select a.publicAddress).ToList();

                int amountOfAddresses = addresses.Count();
                string address = addresses[random.Next(amountOfAddresses)];
                return address;
            }
            catch (NullReferenceException n)
            {
                Console.WriteLine(n.Message);
                throw;
            }
        }

        private int GetRandomId()
        {
            int randomNumber = random.Next(99999999);

            bool unique = false;
            while (!unique)
            {
                if (GeneratedTransactionIdIsUnique(randomNumber))
                {
                    unique = true;
                }
                else
                {
                    randomNumber = random.Next(99999999);
                }
            }
            return randomNumber;
        }

        // Returns all Wallet addresses from the Wallets Table in the database
        public IEnumerable<Wallet> GetAllWallets()
        {
            return _context.Wallets.ToList<Wallet>();
        }

        // Returns all transaction Ids from the transactions Table in the database
        private List<int> GetAllTransactionIds()
        {
            var transactionIds = (
                from t in _context.Transactions
                orderby t.Id
                select t.Id).ToList();

            return transactionIds;
        }

        public IEnumerable<Block> GetAllBlocks()
        {
            try
            {
                return _context.Blocks.ToList();
            }
            catch (ArgumentNullException)
            {
                throw;
            }
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            try
            {
                return _context.Transactions.ToList();
            }
            catch (ArgumentNullException)
            {
                throw;
            }
        }

        private Block GetLatestBlock()
        {
            var lastBlockId = _context.Blocks.Max(x => x.Id);
            return _context.Blocks.FirstOrDefault(x => x.Id == lastBlockId);
        }

        // Every transaction created, is stored in a PendingTransactions List.
        public Transaction CreateTransaction(string fromAddress, string toAddress, decimal amount)
        {
            Transaction transaction = new Transaction();

            if (CheckForEnoughBalance(fromAddress, amount))
            {
                //transaction.Id = GetRandomId();
                transaction.TimeStamp = DateTime.Now;
                transaction.FromAddress = fromAddress;
                transaction.ToAddress = toAddress;
                transaction.Amount = amount;
                transaction.RewardTrx = false;
                transaction.Processed = false;

                // Store transaction in database to be processed later
                StoreTransactionInDatabase(transaction);

                //// Adds new transaction to the PendingTransactions list
                //PendingTransactions.Add(transaction);

                _notyf.Success("FlipCoin Transaction created and added to the database");
            }
            else
            {
                var username = (
                from a in _context.Wallets
                where a.publicAddress == fromAddress
                select a.Username).FirstOrDefault();

                _notyf.Error($"Not enough balance on fromAddress from User: {username}, Creation of transaction is aborted.");
            }

            return transaction;
        }

        public void CreateRewardTransaction(string toAddress)
        {
            Transaction rewardTransaction = new Transaction();
            //rewardTransaction.Id = GetRandomId();
            rewardTransaction.TimeStamp = DateTime.Now;
            rewardTransaction.FromAddress = null;
            rewardTransaction.ToAddress = toAddress;
            rewardTransaction.Amount = Reward;
            rewardTransaction.RewardTrx = true;
            rewardTransaction.Processed = false;

            // Adds new reward transaction to the database, to be processed later.
            StoreTransactionInDatabase(rewardTransaction);

            _notyf.Success("Reward transaction created.");
        }

        private bool CheckForEnoughBalance(string fromAddress, decimal amount)
        {
            var balance = (
                from a in _context.Wallets
                where a.publicAddress == fromAddress
                select a.Balance).FirstOrDefault();

            return balance >= amount ? true : false;
        }

        public decimal CheckBalance(string fromAddress, decimal amount)
        {
            var balance = (
                from a in _context.Wallets
                where a.publicAddress == fromAddress
                select a.Balance).FirstOrDefault();

            return balance;
        }

        private Block Mine(Block newBlock, int difficulty)
        {
            var leadingZeros = new string('0', difficulty);

            while (newBlock.Hash == null || newBlock.Hash.Substring(0, difficulty) != leadingZeros)
            {
                newBlock.Nonce++;
                newBlock.Hash = CalculateHash(
                    newBlock.TimeStamp,
                    newBlock.PreviousHash,
                    newBlock.Transactions,
                    newBlock.Nonce);
            }

            _notyf.Information("Mining process completed.");
            return newBlock;
        }
        private Block CreateNewBlock(List<Transaction> PendingTransactions)
        {
            // Create the new block to be mined
            Block newBlock = new Block();
            newBlock.TimeStamp = DateTime.Now;
            newBlock.PreviousHash = GetLatestBlock().Hash;
            newBlock.Transactions = PendingTransactions;
            newBlock.Nonce = 0;
            newBlock.Hash = CalculateHash(
                newBlock.TimeStamp,
                newBlock.PreviousHash,
                PendingTransactions,
                newBlock.Nonce);

            // Mine the new block locally (Normally in a P2P environment)
            Block newBlockMined = Mine(newBlock, Difficulty);

            return newBlockMined;
        }

        private void UpdateWalletBalances(Transaction transaction)
        {
            // Update balance of receiving wallet
            string UpdateAddressTo = transaction.ToAddress;

            var receivingWallet = (
                from w in _context.Wallets
                where w.publicAddress == UpdateAddressTo
                select w).FirstOrDefault();
            
            if (transaction.Processed == true)
            {
                receivingWallet.Balance += transaction.Amount;
                _context.Update(receivingWallet);
                _context.SaveChanges();
            }
            else
            {
                _notyf.Warning("Transaction not yet processed, RECEIVING wallet will not be updated.");
            }

            // Update balance of sending wallet
            if (transaction.FromAddress == null)
            {
                _notyf.Information("There is no From Address, updating of From Address is skipped.");
            }
            else
            {
                string UpdateAddressFrom = transaction.FromAddress;
                
                var sendingWallet = (
                    from w in _context.Wallets
                    where w.publicAddress == UpdateAddressFrom
                    select w).FirstOrDefault();
                
                if (transaction.Processed == true)
                {
                    sendingWallet.Balance -= transaction.Amount;
                    _context.Update(sendingWallet);
                    _context.SaveChanges();
                }
                else
                {
                    _notyf.Warning("Transaction not yet processed, SENDING wallet will not be updated.");
                }
            }
            _notyf.Information("Wallet balances updated.");
        }

        /* 
         * Processes the pending transactions in a block and verifies it.
         * the minerAddress parameter will be a randomly chosen address, 
         * from the Wallet table to simulate a P2P network.
         */
        public void ProcessPendingTransactions()
        {
            // Creates List of transactions that have not been processed
            var PendingTransactions = (
                        from r in _context.Transactions
                        where r.Processed == false
                        select r).ToList();

            bool isEmpty = !PendingTransactions.Any();
            if (isEmpty)
            {
                _notyf.Warning("There are no transactions to process!", 10);
            }
            else
            {
                // Add the Reward transaction (created after adding a new Block to the database) to PendingTransactions list
                // Otherwise it falls through the cracks and you lose it.
                var rewardTransaction = (
                        from r in _context.Transactions
                        where r.RewardTrx == true && r.Processed == false
                        select r).FirstOrDefault();

                if (rewardTransaction != null)
                {
                    PendingTransactions.Add(rewardTransaction);
                }
                else
                {
                    _notyf.Information("No reward transaction available, continuing...");
                }

                // Process all transactions in PendingTransactions list
                foreach (Transaction transaction in PendingTransactions)
                {
                    transaction.Processed = true;
                    if (transaction.RewardTrx == true)
                        UpdateTransactionInDatabase(transaction);
                    UpdateWalletBalances(transaction);
                }

                // Add new block to database (Processed flags should all be TRUE)
                AddBlock(CreateNewBlock(PendingTransactions));

                // Clear PendingTransactions list, 
                // after the block with new transactions has been added to database
                PendingTransactions.Clear();

                _notyf.Success("Pending transactions processed!", 10);
            }
        }

        private void AddBlock(Block block)
        {
            StoreBlockInDatabase(block);

            // Reward transaction for the Miner that adds the block to the Chain.
            _notyf.Information("New Block created and added to the database.", 10);
            CreateRewardTransaction(GetRandomWalletAddress());
        }

        private void StoreBlockInDatabase(Block newBlock)
        {
            _context.Blocks.Add(newBlock);
            _context.SaveChanges();
        }

        private void StoreTransactionInDatabase(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        private void UpdateTransactionInDatabase(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }

        public void DepositFundsToWallet(string walletAddress, decimal amount)
        {
            var wallet = (
                from w in _context.Wallets
                where w.publicAddress == walletAddress
                select w).FirstOrDefault();

            wallet.Balance += amount;

            _context.Wallets.Update(wallet);
            _context.SaveChanges();
            _notyf.Success($"Amount {amount} has been deposited to wallet {wallet.publicAddress}");
        }

        public void WithdrawFundsFromWallet(string walletAddress, decimal amount)
        {
            var wallet = (
                from w in _context.Wallets
                where w.publicAddress == walletAddress
                select w).FirstOrDefault();

            wallet.Balance -= amount;

            _context.Wallets.Update(wallet);
            _context.SaveChanges();
            _notyf.Success($"Amount {amount} has been withdawn from wallet {wallet.publicAddress}");
        }
    }
}