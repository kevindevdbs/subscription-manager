using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SubscriptionManager.Application.DTOs;
using SubscriptionManager.Domain.Exceptions;

namespace SubscriptionManager.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is SubscriptionManagerException subscriptionManagerException)
        {
            context.HttpContext.Response.StatusCode = (int)subscriptionManagerException.GetStatusCode();

            context.Result = new ObjectResult(new ResponseErrorJson(subscriptionManagerException.GetErrorMessages()));
        }
        else
        {
            _logger.LogError(
                context.Exception,
                "Erro não tratado em {Method} {Path}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Result = new ObjectResult(new ResponseErrorJson("Erro desconhecido."));
        }
    }
}
