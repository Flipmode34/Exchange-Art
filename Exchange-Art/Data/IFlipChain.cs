using System.Collections.Generic;
using Exchange_Art.Models;

namespace Exchange_Art.Data
{
    public interface IFlipChain
    {
        IEnumerable<Block> GetAllBlocks();
        IEnumerable<Transaction> GetAllTransactions();
        IEnumerable<Wallet> GetAllWallets();
        void AddGenesisBlock();
        string GetWalletAddress(string username);
        Transaction CreateTransaction(string fromAddress, string toAddress, decimal amount);
        void CreateRewardTransaction(string toAddress);
        void ProcessPendingTransactions();
        Wallet CreateAndStoreWalletInDatabase(string username, string emailaddress);
        void DeleteWalletFromDatabase(int Id);
        decimal CheckBalance(string fromAddress, decimal amount);
        void DepositFundsToWallet(string walletAddress, decimal amount);
        void WithdrawFundsFromWallet(string walletAddress, decimal amount);
    }
}
