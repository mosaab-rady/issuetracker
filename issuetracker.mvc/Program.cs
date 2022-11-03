using issuetracker.Database;
using issuetracker.Email;
using issuetracker.Entities;
using issuetracker.Hubs;
using issuetracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


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

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.Run();
