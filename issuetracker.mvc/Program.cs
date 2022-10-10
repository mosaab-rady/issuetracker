using issuetracker.Database;
using issuetracker.Entities;
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
})
.AddEntityFrameworkStores<PostgresContext>()
.AddDefaultTokenProviders();


builder.Services.AddTransient<IProjectsService, ProjectsService>();
builder.Services.AddTransient<IIssuesService, IssuesService>();
builder.Services.AddTransient<ITagsServices, TagsService>();
builder.Services.AddTransient<IPriorityService, PriorityService>();
builder.Services.AddTransient<IcommentsService, CommentsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
