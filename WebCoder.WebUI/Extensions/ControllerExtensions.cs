using Microsoft.AspNetCore.Mvc;
using WebCoder.WebUI.ViewModels.Common;

namespace WebCoder.WebUI.Extensions;

public static class ControllerExtensions
{
    public static IActionResult NotFoundView(this Controller controller, string message)
    {
        controller.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return controller.View("NotFound", new NotFoundViewModel { Message = message });
    } 
}