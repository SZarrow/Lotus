using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lotus.Scheduler.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Lotus.Scheduler.Web.Services
{
    public class JobDbContext : DbContext
    {
        public JobDbContext()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=jobs.db");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<JobItem> JobItems { get; set; }
    }
}
