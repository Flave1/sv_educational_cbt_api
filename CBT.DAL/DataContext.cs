﻿using CBT.DAL.Models.Authentication;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Questions;
using CBT.DAL.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CBT.DAL
{
    public class DataContext: IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor accessor;
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor accessor)
            : base(options)
        {
            this.accessor = accessor;
        }

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
            var smsClientId = accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "smsClientId")?.Value ?? "";
            var clientId = accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Deleted = false;
                    entry.Entity.CreatedOn = GetCurrentLocalDateTime();
                    entry.Entity.CreatedBy = clientId;
                    entry.Entity.UserType = string.IsNullOrEmpty(clientId) ? 1 : 0;
                    entry.Entity.ClientId = Guid.Parse(clientId);
                    entry.Entity.SmsClientId = smsClientId;
                }
                else
                {
                    entry.Entity.UpdatedOn = GetCurrentLocalDateTime();
                    entry.Entity.UpdatedBy = clientId;
                    entry.Entity.UserType = string.IsNullOrEmpty(clientId) ? 1 : 0;
                    entry.Entity.ClientId = Guid.Parse(clientId);
                    entry.Entity.SmsClientId = smsClientId;
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var smsClientId = accessor?.HttpContext?.Items["smsClientId"]?.ToString() ?? "";
            var clientId = accessor?.HttpContext?.Items["userId"]?.ToString() ?? "";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Deleted = false;
                    entry.Entity.CreatedOn = GetCurrentLocalDateTime();
                    entry.Entity.CreatedBy = clientId;
                    entry.Entity.UserType = string.IsNullOrEmpty(smsClientId) ? 1 : 0;
                    entry.Entity.ClientId = clientId != "" ? Guid.Parse(clientId) : Guid.Empty;
                    entry.Entity.SmsClientId = smsClientId;
                }
                else
                {
                    entry.Entity.UpdatedOn = GetCurrentLocalDateTime();
                    entry.Entity.UpdatedBy = clientId;
                    entry.Entity.UserType = string.IsNullOrEmpty(smsClientId) ? 1 : 0;
                    entry.Entity.ClientId = clientId != "" ? Guid.Parse(clientId) : Guid.Empty;
                    entry.Entity.SmsClientId = smsClientId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public DateTime GetCurrentLocalDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "W. Central Africa Standard Time");
            return localTime;
        }
    }
}
