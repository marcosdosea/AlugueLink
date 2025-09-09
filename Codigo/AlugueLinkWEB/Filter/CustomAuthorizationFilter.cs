using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlugueLinkWEB.Filter
{
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Verifica se a action possui [AllowAnonymous]
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (allowAnonymous)
            {
                return;
            }

            // Verifica se o usuário está autenticado
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                // Se não estiver autenticado, redireciona para login
                context.Result = new RedirectToPageResult("/Identity/Account/Login", 
                    new { area = "Identity", returnUrl = context.HttpContext.Request.Path });
            }
        }
    }
}