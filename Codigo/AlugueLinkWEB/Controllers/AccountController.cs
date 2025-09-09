using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlugueLinkWEB.Areas.Identity.Data;
using AlugueLinkWEB.Models;
using Microsoft.EntityFrameworkCore;

namespace AlugueLinkWEB.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<UsuarioIdentity> userManager,
            SignInManager<UsuarioIdentity> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                NomeCompleto = user.NomeCompleto,
                DataCadastro = user.DataCadastro,
                UltimoLogin = user.UltimoLogin,
                DataNascimento = user.DataNascimento
            };

            return View(model);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            user.NomeCompleto = model.NomeCompleto;
            user.DataNascimento = model.DataNascimento;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("Usuário {Email} alterou sua senha.", user.Email);
                TempData["SuccessMessage"] = "Senha alterada com sucesso!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Account/Users (Admin only - futura implementação)
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.NomeCompleto)
                .ToListAsync();

            return View(users);
        }
    }
}