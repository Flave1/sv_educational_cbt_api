using CBT.DAL.Models.Authentication;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Question;
using CBT.DAL.Models.Setting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CBT.DAL
{
    public class DataContext: IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DataContext()
        {

        }

        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<CandidateCategory> CandidateCategory { get; set; }
        public DbSet<CandidateAnswer> CandidateAnswer { get; set; }
        public DbSet<Examination> Examination { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Setting> Setting { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var loggedInUserId = ""; // _accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "useradmin";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Deleted = false;
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.CreatedBy = loggedInUserId;
                }
                else
                {
                    entry.Entity.UpdatedOn = DateTime.Now;
                    entry.Entity.UpdatedBy = loggedInUserId;
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var loggedInUserId = ""; // _accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "useradmin";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Deleted = false;
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.CreatedBy = loggedInUserId;
                }
                else
                {
                    entry.Entity.UpdatedOn = DateTime.Now;
                    entry.Entity.UpdatedBy = loggedInUserId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
