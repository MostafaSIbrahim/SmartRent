using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRental.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRental.Data.Configuration
{
    public class TenantConfigurations : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasOne(t => t.University)
                    .WithMany(u => u.Tenants)
                    .HasForeignKey(t => t.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);
             builder.HasMany(t => t.RentalContracts)
                    .WithOne(rc => rc.Tenant)
                    .HasForeignKey(rc => rc.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
