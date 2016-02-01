using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using MvcPhoenix.Models;

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