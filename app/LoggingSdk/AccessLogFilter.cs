using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoggingSdk;

public class AccessLogFilter(ILogger<AccessLogFilter> logger) : ActionFilterAttribute
{
    // Choose either Sync or Async 
    // public override void OnActionExecuting(ActionExecutingContext context)
    // {
    //     var controller = context.RouteData.Values["controller"]!.ToString();
    //     var action = context.RouteData.Values["action"]!.ToString();
    //     var requestContent = string.Join(';', context.ActionArguments.Select(x => $"{x.Key}:{JsonSerializer.Serialize(x.Value)}"));
    //     logger.LogInformation("OnActionExecuting: {Controller} {Action} {RequestContent}", controller, action, requestContent);
    // }
    //
    // public override void OnActionExecuted(ActionExecutedContext context)
    // {
    //     var result = context.Result switch
    //     {
    //         ObjectResult objectResult => JsonSerializer.Serialize(objectResult.Value),
    //         ContentResult contentResult => contentResult.Content!,
    //         JsonResult jsonResult => JsonSerializer.Serialize(jsonResult.Value),
    //         _ => context.Result?.ToString() ?? string.Empty
    //     };
    //
    //     logger.LogInformation("OnActionExecuted: {result}", result);
    // }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.RouteData.Values["controller"]!.ToString();
        var action = context.RouteData.Values["action"]!.ToString();
        var requestContent = string.Join(';', context.ActionArguments.Select(x => $"{x.Key}:{JsonSerializer.Serialize(x.Value)}"));
        logger.LogInformation("OnActionExecutingAsync: {controller} {action} {RequestContent}", controller, action, requestContent);
        
        var actionExecutedContext = await next();
        
        var result = actionExecutedContext.Result switch
        {
            ObjectResult objectResult => JsonSerializer.Serialize(objectResult.Value),
            ContentResult contentResult => contentResult.Content!,
            JsonResult jsonResult => JsonSerializer.Serialize(jsonResult.Value),
            _ => context.Result?.ToString() ?? string.Empty
        };
        logger.LogInformation("OnActionExecutedAsync: {result}", result);
    }
}