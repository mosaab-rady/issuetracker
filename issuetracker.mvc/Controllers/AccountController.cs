using System.Security.Claims;
using issuetracker.Email;
using issuetracker.Entities;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace issuetracker.mvc.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<AppUser> userManager;
	private readonly SignInManager<AppUser> signInManager;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly IEmailSender emailSender;

	public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.roleManager = roleManager;
		this.emailSender = emailSender;
	}


	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}


	[AcceptVerbs("Get", "Post")]
	public async Task<IActionResult> IsEmailNotUsed(string email)
	{
		AppUser user = await userManager.FindByEmailAsync(email);

		if (user == null) return Json(true);
		return Json($"The Email ({email}) is used. Please use another email value.");

	}


	[HttpPost]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		string image = await ProcessUploadImage(model.Image, $"{model.FirstName}-{model.LastName}");

		if (!ModelState.IsValid) return View(model);

		AppUser user = new()
		{
			FirstName = model.FirstName,
			LastName = model.LastName,
			Email = model.Email,
			UserName = model.Email,
			Image = image
		};

		var result = await userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			// delete image if not successded
			DeleteImage(image);
			foreach (var err in result.Errors)
			{
				ModelState.AddModelError("", err.Description);
			}
			return View(model);
		}



		try
		{
			await userManager.AddToRoleAsync(user, "member");
		}
		catch (System.InvalidOperationException)
		{
			IdentityRole role = new() { Name = "member" };
			await roleManager.CreateAsync(role);
			await userManager.AddToRoleAsync(user, "member");
		}

		var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

		var confiramtionLink = Url.Action("EmailConfirmed",
																		"Account",
																		new { userId = user.Id, token = token },
																		Request.Scheme);


		// send Email
		await emailSender.SendConfirmationLink(user.Email, confiramtionLink);

		return View("PleaseConfirmEmail");

		// await signInManager.SignInAsync(user, isPersistent: false);

		// return RedirectToAction("index", "home");

	}

	[HttpGet]
	public async Task<IActionResult> EmailConfirmed(string userId, string token)
	{
		if (userId == null || token == null)
		{
			ViewBag.Error = "something went wrong please contact the site owner.";
			return View("NotFound");
		}

		var user = await userManager.FindByIdAsync(userId);

		if (user is null)
		{
			ViewBag.Error = $"No User found";
			return View("NotFound");
		}

		var result = await userManager.ConfirmEmailAsync(user, token);

		if (!result.Succeeded)
		{
			ViewBag.Error = "Email can not be confirmed";
			return View("NotFound");
		}

		await signInManager.SignInAsync(user, isPersistent: false);

		return View();
	}

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		AppUser user = await userManager.FindByEmailAsync(model.Email);
		if (user == null)
		{
			ModelState.AddModelError("", "Invalid Email or Password.");
			return View(model);
		}

		if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
		{
			ModelState.AddModelError("", "Email not confirmed yet.");
			return View(model);
		}

		var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);


		if (!result.Succeeded)
		{
			ModelState.AddModelError("", "Invalid Email or Password.");
			return View(model);
		}

		if (await userManager.IsInRoleAsync(user, "manager"))
		{
			return RedirectToAction("index", "home");
		}

		return RedirectToAction("AssignedToMe", "issues");

	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();

		return RedirectToAction("index", "home");
	}


	[HttpGet]
	public IActionResult AccessDenied()
	{
		ViewBag.Title = "Access Denied";
		ViewBag.Text = "You do not have permission to access this resource";
		return View();
	}


	private async Task<string> ProcessUploadImage(IFormFile model, string name)
	{
		if (!model.ContentType.StartsWith("image"))
		{
			ModelState.AddModelError("Image", $"Image field must be an Image.");
			return name;
		}

		string uniqueFileName = $"{name}-{DateTimeOffset.UtcNow.Ticks}.jpeg";
		string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", uniqueFileName);

		using var image = Image.Load(model.OpenReadStream());
		image.Mutate(img => img.Resize(250, 250));
		await image.SaveAsJpegAsync(FilePath);

		return uniqueFileName;
	}

	private void DeleteImage(string filename)
	{
		string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
		System.IO.File.Delete(filePath);
	}


}