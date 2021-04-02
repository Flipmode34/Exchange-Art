using System.Threading.Tasks;
using Exchange_Art.Models;
using Exchange_Art.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Globalization;

namespace Exchange_Art.Controllers
{
    public class WalletController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private UserManager<ApplicationUser> _userManager;
        private readonly IFlipChain _flipChain;

        [ActivatorUtilitiesConstructor] // Marks this contructor as the primary one for DI.
        public WalletController(ApplicationDbContext context, INotyfService notyf, UserManager<ApplicationUser> userManager, IFlipChain flipChain)
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;
            _flipChain = flipChain;
        }

        public async Task<IActionResult> CheckBalance()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);
            // Reference logged in User
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);

            var walletAddress = ( 
                from w in _context.Wallets
                where w.Username == LoggedInUser.UserName
                select w.publicAddress).FirstOrDefault();

            if (walletAddress != null)
            {
                var walletBalance = (
                from w in _context.Wallets
                where w.Username == LoggedInUser.UserName
                select w.Balance).FirstOrDefault();

                _notyf.Information($"Your wallet balance is: {walletBalance}");
            }
            else
            {
                _notyf.Warning("No wallet balance recorded!", 10);
            }

            var wallet = (from w in _context.Wallets
                          where w.Username == LoggedInUser.UserName
                          select w).FirstOrDefault();

            ViewBag.walletAddress = walletAddress;
            return View("Wallet", wallet);
        }
        
        public async Task<IActionResult> CreateWallet()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);
            // Reference logged in User
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);

            var walletAddress = (
                from w in _context.Wallets
                where w.Username == LoggedInUser.UserName
                select w.publicAddress).FirstOrDefault();

            if (walletAddress == null)
            {
                Wallet newWallet = _flipChain.CreateAndStoreWalletInDatabase(LoggedInUser.UserName, LoggedInUser.Email);
                ViewBag.walletAddress = newWallet.publicAddress;
                _notyf.Success($"Wallet {newWallet.publicAddress} created!");
            }
            else
            {
                ViewBag.newWalletAddress = $"Wallet for user {LoggedInUser.UserName} already exists.";
                ViewBag.walletAddress = walletAddress;
                _notyf.Error($"Wallet for user {LoggedInUser.UserName} already exists!");
            }

            var wallet = (from w in _context.Wallets
                          where w.Username == LoggedInUser.UserName
                          select w).FirstOrDefault();
            return View("Wallet", wallet);
        }

        public async Task<IActionResult> DeleteWallet()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);
            // Reference logged in User
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);

            var walletId = (
                from w in _context.Wallets
                where w.Username == LoggedInUser.UserName
                select w.Id).FirstOrDefault();

            var walletAddress = (
                from w in _context.Wallets
                where w.Username == LoggedInUser.UserName
                select w.publicAddress).FirstOrDefault();

            _flipChain.DeleteWalletFromDatabase(walletId);

            Wallet emptyWallet = new Wallet
            {
                Username = LoggedInUser.UserName,
                Email = "NULL",
                publicAddress = "NULL",
                Balance = 0.000M
            };

            if (LoggedInUser != null)
            {
                _notyf.Success($"Wallet {walletAddress} deleted!");
                return View("Wallet", emptyWallet);
            }
            else
                _notyf.Error($"Wallet {walletAddress} not deleted!");
            return RedirectToAction("Wallet", "Wallet");
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET:
        // Wallet info of Logged-in User
        [Authorize]
        public async Task<IActionResult> Wallet()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);

            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);

            Wallet wallet = (from w in _context.Wallets
                          where w.Username == LoggedInUser.UserName
                          select w).FirstOrDefault();

            if (wallet == null)
            {
                _flipChain.CreateAndStoreWalletInDatabase(LoggedInUser.UserName, LoggedInUser.Email);
                wallet = (from w in _context.Wallets
                              where w.Username == LoggedInUser.UserName
                              select w).FirstOrDefault();
            }
            else
            {
                ViewBag.walletAddress = _flipChain.GetWalletAddress(LoggedInUser.UserName);
            }

            return View("Wallet", wallet);
        }

        // GET:
        // Wallet info of all Users
        [Authorize]
        public IActionResult ShowWallets()
        {
            var wallets = (from w in _context.Wallets
                          select w).ToList();

            return View(wallets);
        }

        // GET:
        // Deposit page
        [Authorize]
        public IActionResult Deposit(int Id)
        {
            var wallet = (from w in _context.Wallets
                          where w.Id == Id
                          select w).FirstOrDefault();

            WalletViewModel walletViewModel = new WalletViewModel
            {
                Id = wallet.Id,
                Username = wallet.Username,
                publicAddress = wallet.publicAddress
            };

            return View(walletViewModel);
        }

        public IActionResult DepositFunds(WalletViewModel wallet)
        {

            _flipChain.DepositFundsToWallet(wallet.publicAddress, wallet.Balance);

            var walletObject = (from w in _context.Wallets
                          where w.Id == wallet.Id
                          select w).FirstOrDefault();

            return View("Wallet", walletObject);
        }

        // GET:
        // Withdraw page
        [Authorize]
        public IActionResult Withdraw(int Id)
        {
            var wallet = (from w in _context.Wallets
                          where w.Id == Id
                          select w).FirstOrDefault();

            WalletViewModel walletViewModel = new WalletViewModel
            {
                Id = wallet.Id,
                Username = wallet.Username,
                publicAddress = wallet.publicAddress
            };

            return View(walletViewModel);
        }

        public IActionResult WithdrawFunds(WalletViewModel wallet)
        {

            _flipChain.WithdrawFundsFromWallet(wallet.publicAddress, wallet.Balance);

            var walletObject = (from w in _context.Wallets
                                where w.Id == wallet.Id
                                select w).FirstOrDefault();

            return View("Wallet", walletObject);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("Wallet", error.Description);
        }
    }
}