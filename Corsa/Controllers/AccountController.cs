using System.Threading.Tasks;
using Corsa.Domain.Models;
using Corsa.Domain.Models.Requests;
using Corsa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace Corsa.Controllers
{
    public class AccountController : Controller
    {
        private ISourceRepository _repository;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private IPasswordHasher<IdentityUser> _passwordHasher;

        public AccountController(ISourceRepository repository, IConfiguration configuration,
            UserManager<IdentityUser> userMgr,SignInManager<IdentityUser> signInMgr, IPasswordHasher<IdentityUser> passwordHash)
        {
            _repository = repository;
            _userManager = userMgr;
            _signInManager = signInMgr;
            _passwordHasher = passwordHash;            
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginViewModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user =
                await _userManager.FindByNameAsync(loginModel.Name);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    if ((await _signInManager.PasswordSignInAsync(user,
                    loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect(loginModel?.ReturnUrl ?? Url.Action("Requests","Request"));
                    }
                }
            }

            ModelState.AddModelError("", "Invalid name or password");
            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
               
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {                    
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    _repository.AddUserSettings(new Domain.Models.Account.UserSettings());
                    return RedirectToAction("Index","Project");
                }
                AddErrors(result);
            }

            return View(model);
        }
    
        private void AddErrors(IdentityResult result)
        {
            

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}