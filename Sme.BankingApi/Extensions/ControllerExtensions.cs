using Microsoft.AspNetCore.Mvc;
using Sme.BankingApi.Commands.Common;

namespace Sme.BankingApi.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult CommandToHttpResult(this ControllerBase controller, CommandResult result)
        {
            if (result.Success)
            {
                return controller.Ok();
            }
            else
            {
                return controller.BadRequest(result.Errors);
            }
        }

        public static IActionResult CommandToHttpResult<T>(this ControllerBase controller, CommandResult<T> result)
        {
            if (result.Success)
            {
                return controller.Ok(result.Data);
            }
            else
            {
                return controller.BadRequest(result.Errors);
            }
        }
    }
}
