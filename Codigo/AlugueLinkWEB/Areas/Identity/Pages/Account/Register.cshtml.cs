// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AlugueLinkWEB.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace AlugueLinkWEB.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IUserStore<UsuarioIdentity> _userStore;
        private readonly IUserEmailStore<UsuarioIdentity> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<UsuarioIdentity> userManager,
            IUserStore<UsuarioIdentity> userStore,
            SignInManager<UsuarioIdentity> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Email � obrigat�rio")]
            [EmailAddress(ErrorMessage = "Email inv�lido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     Nome completo do usu�rio
            /// </summary>
            [Required(ErrorMessage = "Nome completo � obrigat�rio")]
            [StringLength(100, ErrorMessage = "O nome deve ter no m�ximo 100 caracteres")]
            [Display(Name = "Nome Completo")]
            public string NomeCompleto { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Senha � obrigat�ria")]
            [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no m�ximo {1} caracteres.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infraestrutura and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Password", ErrorMessage = "As senhas n�o coincidem.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                // Verificar se j� existe usu�rio com este email
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, $"J� existe uma conta cadastrada com o email {Input.Email}. Tente fazer login ou use outro email.");
                    return Page();
                }

                var user = CreateUser();

                // No ASP.NET Core Identity, � padr�o usar o email como username
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                
                // Configurar dados adicionais do usu�rio
                user.NomeCompleto = Input.NomeCompleto;
                user.DataCadastro = DateTime.Now;
                user.Ativo = true;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usu�rio {Email} criou uma nova conta com senha.", Input.Email);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    try
                    {
                        await _emailSender.SendEmailAsync(Input.Email, "Confirme seu email - AlugueLink",
                            $"<h2>Bem-vindo ao AlugueLink, {Input.NomeCompleto}!</h2>" +
                            $"<p>Sua conta foi criada com sucesso. Com ela voc� pode tanto cadastrar im�veis para alugar quanto procurar im�veis para alugar.</p>" +
                            $"<p>Por favor, confirme sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.</p>" +
                            $"<p>Se voc� n�o conseguir clicar no link, copie e cole o seguinte endere�o no seu navegador:</p>" +
                            $"<p>{HtmlEncoder.Default.Encode(callbackUrl)}</p>" +
                            $"<br/><p>Atenciosamente,<br/>Equipe AlugueLink</p>");
                        
                        _logger.LogInformation("Email de confirma��o enviado para {Email}", Input.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao enviar email de confirma��o para {Email}", Input.Email);
                        // Continua o processo mesmo se o email falhar - o usu�rio ainda pode usar a conta
                        TempData["EmailWarning"] = "N�o foi poss�vel enviar o email de confirma��o, mas sua conta foi criada com sucesso.";
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // Atualizar �ltimo login
                        user.UltimoLogin = DateTime.Now;
                        await _userManager.UpdateAsync(user);
                        
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("Usu�rio {Email} fez login automaticamente ap�s registro.", Input.Email);
                        
                        TempData["SuccessMessage"] = $"Bem-vindo ao AlugueLink, {Input.NomeCompleto}! Sua conta foi criada com sucesso.";
                        return LocalRedirect(returnUrl);
                    }
                }
                
                // Tratar erros espec�ficos de forma mais amig�vel
                foreach (var error in result.Errors)
                {
                    string friendlyError = error.Description;
                    
                    // Traduzir erros comuns para portugu�s
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            friendlyError = "Este email j� est� sendo usado por outra conta.";
                            break;
                        case "DuplicateEmail":
                            friendlyError = "Este email j� est� sendo usado por outra conta.";
                            break;
                        case "PasswordTooShort":
                            friendlyError = "A senha deve ter pelo menos 8 caracteres.";
                            break;
                        case "PasswordRequiresDigit":
                            friendlyError = "A senha deve conter pelo menos um n�mero.";
                            break;
                        case "PasswordRequiresLower":
                            friendlyError = "A senha deve conter pelo menos uma letra min�scula.";
                            break;
                        case "PasswordRequiresUpper":
                            friendlyError = "A senha deve conter pelo menos uma letra mai�scula.";
                            break;
                        case "InvalidEmail":
                            friendlyError = "Email inv�lido.";
                            break;
                        default:
                            _logger.LogWarning("Erro n�o tratado no registro: {Code} - {Description}", error.Code, error.Description);
                            break;
                    }
                    
                    ModelState.AddModelError(string.Empty, friendlyError);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private UsuarioIdentity CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UsuarioIdentity>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UsuarioIdentity)}'. " +
                    $"Ensure that '{nameof(UsuarioIdentity)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UsuarioIdentity> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UsuarioIdentity>)_userStore;
        }
    }
}
