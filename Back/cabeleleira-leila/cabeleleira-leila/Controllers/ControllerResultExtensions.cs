using cabeleleira_leila.Models;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

internal static class ControllerResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, OperationResult result)
    {
        return controller.StatusCode((int)result.StatusCode, result);
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, OperationResult<T> result)
    {
        return controller.StatusCode((int)result.StatusCode, result);
    }
}
