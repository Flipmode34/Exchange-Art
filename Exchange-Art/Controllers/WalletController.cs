using System;
using System.Threading.Tasks;
using Exchange_Art.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Exchange_Art.Controllers
{
    public class WalletController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        [ActivatorUtilitiesConstructor] // Marks this contructor as the primary one for DI.
        public WalletController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public WalletController()
        {

        }

        // Has parameter Web3.Accounts.Account
        public async Task<decimal> GetAccountBalance(Account LeaseRequesterAccount)
        {
            // Reference account and Ropsten Infura Ethereum test environment
            var web3 = new Web3(LeaseRequesterAccount, "https://ropsten.infura.io/v3/87f3fd59965241539d6721a231635ea0");

            // Get Wei balance of a public address
            var WeiBalance = await web3.Eth.GetBalance.SendRequestAsync(LeaseRequesterAccount.Address);
            
            // Get Eth balance of a public address
            var EtherAmount = Web3.Convert.FromWei(WeiBalance.Value);

            return EtherAmount;
        }

        // Has parameter string, for "Check Balance" button in wallet page of user
        public async Task<IActionResult> CheckBalance(string Id)
        {
            // Reference LeaseRequester and Owner of the ArtPiece
            ApplicationUser LoggedInuser = await _userManager.FindByIdAsync(Id);

            // Reference Ethereum account for balance checking
            var LeaseRequesterAccount = new Account(LoggedInuser.WalletPrivateKey);
            var web3 = new Web3(LeaseRequesterAccount, "https://ropsten.infura.io/v3/87f3fd59965241539d6721a231635ea0");

            // Get Wei balance of a public address
            var WeiBalance = await web3.Eth.GetBalance.SendRequestAsync(LeaseRequesterAccount.Address);

            // Get Eth balance of a public address
            var EtherBalance = Web3.Convert.FromWei(WeiBalance.Value);

            ViewBag.EthBalance = $"Your Ethereum account balance is: {EtherBalance} Ether.";

            return View("Wallet", LoggedInuser);
        }

        public async Task<IActionResult> CheckTransactions(string Id)
        {
            // Reference LeaseRequester and Owner of the ArtPiece
            ApplicationUser LoggedInuser = await _userManager.FindByIdAsync(Id);

            // Reference Ethereum account for transactions checking
            var TransactionsCheckAccount = new Account(LoggedInuser.WalletPrivateKey);
            var web3 = new Web3(TransactionsCheckAccount, "https://ropsten.infura.io/v3/87f3fd59965241539d6721a231635ea0");

            // Get transactions count from Ethereum test network
            var TransactionCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(TransactionsCheckAccount.Address);

            ViewBag.TransactionCount = $"Your Ethereum Transaction count is: {TransactionCount}";

            return View("Wallet", LoggedInuser);
        }
        
        public async Task<IActionResult> CreateWallet(string Id)
        {
            // Reference logged in User
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(Id);

            if (LoggedInUser != null)
            {
                var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
                var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                var account = new Account(privateKey); // Creation of Eth account + public key

                if (LoggedInUser.WalletPrivateKey == null && LoggedInUser.EtheruemAddress == null)
                {
                    // Set private key of new Eth wallet
                    LoggedInUser.WalletPrivateKey = privateKey;
                    // Set public Eth address of logged in User
                    LoggedInUser.EtheruemAddress = account.Address;

                    ViewBag.Message = "";

                    IdentityResult result = await _userManager.UpdateAsync(LoggedInUser);
                    if (result.Succeeded)
                        return RedirectToAction("Wallet", LoggedInUser);
                    else
                        Errors(result);
                }
                else
                {
                    ViewBag.WalletCreated = "There is already a Ethereum wallet created for this user.";
                }
            }
            else
                ModelState.AddModelError("Wallet", "User Not Found");

            return View("Wallet", LoggedInUser);
        }

        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferEther(string PrivateKey, string ToAddress, decimal EthAmount)
        {
            var account = new Account(PrivateKey);
            var web3 = new Web3(account, "https://ropsten.infura.io/v3/87f3fd59965241539d6721a231635ea0"); // Web3 account in Eth test env
            var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(ToAddress, EthAmount);
            
            return transaction;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET:
        // Wallet info of Logged-in User
        [Authorize]
        public async Task<IActionResult> Wallet(string id)
        {
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(id);

            var UserId = _userManager.GetUserId(User);

            // Check that the user that is trying to access the update profile page,
            // is the correct user.
            if (LoggedInUser.Id == UserId)
                return View(LoggedInUser);
            else

                return RedirectToAction("Index");
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("Wallet", error.Description);
        }
    }

}