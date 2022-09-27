using System.Security.Claims;
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

	public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
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

		await userManager.AddToRoleAsync(user, "member");

		await signInManager.SignInAsync(user, isPersistent: false);

		return RedirectToAction("index", "home");

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

		var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);


		if (!result.Succeeded)
		{
			ModelState.AddModelError("", "Invalid Email or Password.");
			return View(model);
		}

		return RedirectToAction("index", "home");

	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();

		return RedirectToAction("index", "home");
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