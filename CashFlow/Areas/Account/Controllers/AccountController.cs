using System;
using System.Linq;
using System.Threading.Tasks;
using CashFlow.Areas.Account.Services;
using CashFlow.Areas.Account.ViewModels;
using CashFlow.Areas.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Areas.Account.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly IUsersService _usersService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, IAccountService accountService, IUsersService usersService, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._accountService = accountService;
            this._usersService = usersService;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }            

            IdentityResult result = await _accountService.AddUser(model.Email, model.Name, model.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {    
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(model);
            }

            AppUser user = await _usersService.FindByEmailAsync(model.Email);
            
            await _accountService.SendConfirmEmail(user);
                       
            return RedirectToAction($"RegisterFinish", $"Account", new { email = user.Email });

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Redirect("/");
            }
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult RegisterFinish(string email)
        {
            ViewBag.email = email;
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Вы не подтвредили email");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                return View(model);
            }


            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return Redirect("/");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public async Task<IActionResult> Edit()
        {

            AppUser currentUser = await _userManager.GetUserAsync(User);
            var editViewModel = new EditViewModel { Email = currentUser.Email, Name = currentUser.Name, RegisterDate = currentUser.RegisterDate };

            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return BadRequest();
            }

            currentUser.Name = model.Name;
            
            var result = await _userManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return Redirect("/");
        }


        public  IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            AppUser currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return BadRequest();
            }
            
            IdentityResult result = await _userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            return Redirect("/");
        }

        public  IActionResult SendConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> SendConfirmEmail(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Такой email не зарегистрирован");
                return View();
            }

           
            await _accountService.SendConfirmEmail(user);

            return RedirectToAction($"RegisterFinish", $"Account", new { email });
        }

        public  IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Такой email не зарегистрирован");
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action(
                $"RecoveyPassword",
                $"Account",
                new { userId = user.Id, code },
                HttpContext.Request.Scheme
            );

            await _accountService.SendRecoveryPasswordEmail(user.Email, callbackUrl);

            return View();
        }

        public  IActionResult RecoveyPassword(string userId, string code)
        {
            return View(new RecoveryPasswordViewModel() {Id = userId, Code = code});
        }

        [HttpPost]
        public async Task<IActionResult> RecoveyChangePassword(RecoveryPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            AppUser user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            return View();
        }

    }


}