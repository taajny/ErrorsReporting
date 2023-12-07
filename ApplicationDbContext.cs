using ErrorsReporting.Models.Configurations;
using ErrorsReporting.Models.Domains;
using System;
using System.Data.Entity;
using System.Linq;

namespace ErrorsReporting
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("name=ApplicationDbContext")
        {
        }

        public DbSet<Error> Errors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ErrorConfiguration());
        }

    }

 }