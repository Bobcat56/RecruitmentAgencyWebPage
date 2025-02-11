using Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class RecruitmentContext : IdentityDbContext<IdentityUser>
    {
        public RecruitmentContext(DbContextOptions<RecruitmentContext> options)
            : base(options)
        {
        }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<EncryptionKey> EncryptionKeys { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CV>()
                .HasOne(cv => cv.UserIdentity)
                .WithMany()
                .HasForeignKey(cv => cv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CV>()
                .HasOne(cv => cv.EmployerIdentity)
                .WithMany()
                .HasForeignKey(cv => cv.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
