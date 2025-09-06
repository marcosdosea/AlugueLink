using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.Service;

namespace AlugueLinkWEB.Filter
{
    /// <summary>
    /// Filtro para tratamento customizado de exce��es
    /// </summary>
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ServiceException serviceException)
            {
                _logger.LogError(serviceException, "Erro de servi�o: {Message}", serviceException.Message);
                
                context.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                        new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                        new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                    {
                        ["ErrorMessage"] = "Ocorreu um erro no processamento da sua solicita��o. Tente novamente."
                    }
                };
                
                context.ExceptionHandled = true;
            }
            else
            {
                _logger.LogError(context.Exception, "Erro n�o tratado: {Message}", context.Exception.Message);
                
                context.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                        new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                        new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                    {
                        ["ErrorMessage"] = "Ocorreu um erro inesperado. Tente novamente mais tarde."
                    }
                };
                
                context.ExceptionHandled = true;
            }
        }
    }
}