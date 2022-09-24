using issuetracker.Database;
using issuetracker.Services;
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


builder.Services.AddTransient<IProjectsService, ProjectsService>();

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

app.UseAuthorization();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
