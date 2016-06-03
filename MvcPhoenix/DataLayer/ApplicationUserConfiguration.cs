using MvcPhoenix.Models;
using System.Data.Entity.ModelConfiguration;

namespace MvcPhoenix.DataLayer
{
    public class ApplicationUserConfiguration : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserConfiguration()
        {
            Property(au => au.FirstName).HasMaxLength(256).IsOptional();
            Property(au => au.LastName).HasMaxLength(256).IsOptional();
            Property(au => au.FaxNumber).HasMaxLength(256).IsOptional();
            Property(au => au.Address).HasMaxLength(256).IsOptional();
            Property(au => au.City).HasMaxLength(256).IsOptional();
            Property(au => au.State).HasMaxLength(256).IsOptional();
            Property(au => au.PostalCode).HasMaxLength(50).IsOptional();
            Property(au => au.Country).HasMaxLength(256).IsOptional();
            Ignore(au => au.RolesList);
        }
    }
}