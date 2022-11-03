using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.mvc.Controllers;

public class ErrorController : Controller
{

	private readonly ILogger<ErrorController> ilogger;

	public ErrorController(ILogger<ErrorController> ilogger)
	{
		this.ilogger = ilogger;
	}

	[Route("Error/{statuscode}")]
	public IActionResult HttpStatusCodeHandler(int statuscode)
	{
		var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
		switch (statuscode)
		{
			case 404:
				ViewBag.Error = "Sorry, The resource you requiested could not be found.";

				break;
		}

		return View("NotFound");
	}


	[Route("Error")]
	public IActionResult Error()
	{

		var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

		ilogger.LogError($"The Path {exceptionDetails.Path} threw an exception {exceptionDetails.Error} ");

		return View("Error");
	}
}