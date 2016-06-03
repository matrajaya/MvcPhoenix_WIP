using Microsoft.AspNet.Identity.EntityFramework;
using MvcPhoenix.Models;
using System.Data.Entity;

namespace MvcPhoenix.DataLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AuthConnection", throwIfV1Schema: false)
        {
        }

        // Comment this block first then rebuild before using EF scafolding; Otherwise error comes up.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ApplicationUserConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}