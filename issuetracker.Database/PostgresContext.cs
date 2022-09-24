using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Database;

public class PostgresContext : DbContext
{
	public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
	{
	}
	protected PostgresContext()
	{
	}


	public DbSet<Project> Projects { get; set; }

	public DbSet<Issue> Issues { get; set; }

	public DbSet<Tag> Tags { get; set; }

	public DbSet<Priority> Priority { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasPostgresExtension("uuid-ossp");


		modelBuilder.Entity<Project>(entity =>
				{
					entity.HasIndex(e => e.Name).IsUnique(true);
					entity.Property(e => e.Id)
					.HasDefaultValueSql("uuid_generate_v4()");
				});


		modelBuilder.Entity<Issue>(entity =>
		{
			entity.Property(e => e.Id)
			.HasDefaultValueSql("uuid_generate_v4()");
		});

		modelBuilder.Entity<Tag>(entity =>
		{
			entity.Property(e => e.Id)
			.HasDefaultValueSql("uuid_generate_v4()");
		});

		modelBuilder.Entity<Priority>(entity =>
		{
			entity.Property(e => e.Id)
			.HasDefaultValueSql("uuid_generate_v4()");
		});

	}
}