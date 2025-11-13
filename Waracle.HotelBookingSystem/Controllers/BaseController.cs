namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseController : Controller
    {
        protected async Task<IActionResult> HandleActionAsync(
            ILogger logger,
            Func<Task<IActionResult>> action,
            string methodName,
            string errorLogMessage)
        {
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"{methodName} operation was cancelled.");
                return StatusCode(499, "Operation cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, errorLogMessage);
                return StatusCode(
                    500,
                    new
                        {
                            error = "Internal Server Error",
                            message = ex.Message
                        });
            }
        }
    }
}
