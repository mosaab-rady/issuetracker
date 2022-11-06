using System.Text.Json.Serialization;
using issuetracker.Api.Helpers;
using issuetracker.Database;
using issuetracker.Email;
using issuetracker.Entities;
using issuetracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


/// Nuget Packeges required
/// 1) $ dotnet add package Microsoft.EntityFrameworkCore.Design
/// 2) $ dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
/// 3)$ dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
/// 4)$ dotnet add package EFCore.NamingConventions
/// 5) dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

builder.Services.AddDbContextPool<PostgresContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetSection("Postgres").Get<PostgresConnection>().ConnectionString)
	.UseSnakeCaseNamingConvention()
	.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
	.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	options.Password.RequireUppercase = false;
	options.User.RequireUniqueEmail = true;
	options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<PostgresContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IIssuesService, IssuesService>();
builder.Services.AddScoped<ITagsServices, TagsService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IcommentsService, CommentsService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.ConfigureApplicationCookie(options =>
{
	options.Events.OnRedirectToLogin = context =>
	{
		context.Response.Redirect("/api/account/notloggedin");
		return Task.CompletedTask;
	};


	options.Events.OnRedirectToAccessDenied = context =>
	{
		context.Response.Redirect("/api/account/accessdenied");
		return Task.CompletedTask;
	};
});


builder.Services.AddAutoMapper(typeof(MappingProfiles));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// app.UseSwagger();
	// app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
