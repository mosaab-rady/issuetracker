using System.Web;
using AutoMapper;
using issuetracker.Aws;
using issuetracker.Dtos;
using issuetracker.Email;
using issuetracker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
	private readonly UserManager<AppUser> userManager;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly SignInManager<AppUser> signInManager;
	private readonly IEmailSender emailSender;
	private readonly IMapper mapper;
	private readonly IConfiguration configuration;
	private readonly IS3Client s3Client;
	public AccountController(
		UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager,
		SignInManager<AppUser> signInManager,
		IEmailSender emailSender,
		IMapper mapper,
		IConfiguration configuration,
		IS3Client s3Client)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
		this.signInManager = signInManager;
		this.emailSender = emailSender;
		this.mapper = mapper;
		this.configuration = configuration;
		this.s3Client = s3Client;
	}

	[HttpGet("isAuthenticted")]
	public async Task<ActionResult<UserDto>> IsAuthenticated()
	{
		if (User.Identity.IsAuthenticated)
		{
			AppUser user = await userManager.FindByEmailAsync(User.Identity.Name);
			UserDto userDto = mapper.Map<UserDto>(user);
			userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

			return userDto;
		}
		return Unauthorized();
	}

	[HttpGet("isEmailUsed")]
	public async Task<Boolean> IsEmailUsed(string email)
	{
		var user = await userManager.FindByEmailAsync(email);

		if (user is null)
		{
			return false;
		}
		return true;
	}

	[HttpPost("login")]
	public async Task<ActionResult<UserDto>> LogIn(LoginDto model)
	{
		var user = await userManager.FindByEmailAsync(model.Email);

		if (user is null)
		{
			return Problem(
			detail: "Invalid Email or Password",
			statusCode: StatusCodes.Status400BadRequest);
		}

		if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
		{
			return Problem(
				detail: "Email not confirmed yet.",
				statusCode: StatusCodes.Status400BadRequest);
		}

		var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

		if (!result.Succeeded)
		{
			return Problem(
				detail: "Invalid Email or Password",
				statusCode: StatusCodes.Status400BadRequest);
		}

		UserDto userDto = mapper.Map<UserDto>(user);
		userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

		return userDto;

	}

	[HttpPost("signup")]
	public async Task<IActionResult> Signup([FromForm] RegisterDto model)
	{

		string imageUrl = "";

		if (model.Image != null)
		{
			if (!model.Image.ContentType.StartsWith("image"))
			{
				return Problem(
					detail: "Image field must be an image.",
					statusCode: StatusCodes.Status400BadRequest);
			}
			imageUrl = await ProcessUploadImage(model.Image, $"{model.FirstName}--{model.LastName}");
		}

		AppUser user = new()
		{
			FirstName = model.FirstName,
			LastName = model.LastName,
			Email = model.Email,
			UserName = model.Email,
			Image = imageUrl
		};

		var result = await userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			if (model.Image != null)
			{
				await DeleteImageAsync(imageUrl);
			}
			List<string> errors = new List<string>();
			foreach (var err in result.Errors)
			{
				errors.Add(err.Description);
			}
			return BadRequest(error: new { errors = errors });
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

		string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

		string code = HttpUtility.UrlEncode(token);

		string url = configuration["UI:Url"];

		string confiramtionLink = $"{url}/email-confirmed?userId={user.Id}&token={code}";


		// var confiramtionLink = Url.Action("EmailConfirmed", "/Account", new { userId = user.Id, token = token }, Request.Scheme);


		// send Email
		await emailSender.SendConfirmationLink(user.Email, confiramtionLink);

		return Ok(new
		{
			statusCode = StatusCodes.Status200OK,
			detail = "thank you for signing up. please confirm your email. we sent you a message."
		});

	}


	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();

		return NoContent();
	}

	// identity redirect url
	[AcceptVerbs("GET", "POST")]
	[Route("notloggedin")]
	public IActionResult NotLoggedIn()
	{
		return Problem(
			detail: "You are not logged in please log in first to get access.",
			statusCode: StatusCodes.Status401Unauthorized);
	}

	[AcceptVerbs("GET", "POST")]
	[Route("AccessDenied")]
	public IActionResult AccessDenied()
	{
		return Problem(
			detail: "You do not have permission to access this resource.",
			statusCode: StatusCodes.Status403Forbidden);
	}


	[HttpGet("EmailConfirmed")]
	public async Task<IActionResult> EmailConfirmed(string userId, string token)
	{
		if (userId == null || token == null)
		{
			return NotFound();
		}

		var user = await userManager.FindByIdAsync(userId);
		if (user is null)
		{
			return NotFound();
		}

		var result = await userManager.ConfirmEmailAsync(user, token);
		if (!result.Succeeded)
		{
			return BadRequest();
		}

		await signInManager.SignInAsync(user, isPersistent: false);

		return Ok(new
		{
			statusCode = StatusCodes.Status200OK,
			detail = "You confirmed your email successfully"
		});
	}


	private async Task<string> ProcessUploadImage(IFormFile model, string name)
	{
		return await s3Client.UploadImage(model, name);

		// string uniqueFileName = $"{name}-{DateTimeOffset.UtcNow.Ticks}.jpeg";
		// string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", uniqueFileName);

		// using var image = Image.Load(model.OpenReadStream());
		// image.Mutate(img => img.Resize(250, 250));
		// await image.SaveAsJpegAsync(FilePath);

		// return uniqueFileName;
	}

	private async Task DeleteImageAsync(string filename)
	{
		await s3Client.DeleteImage(filename);

		// string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
		// System.IO.File.Delete(filePath);
	}

}