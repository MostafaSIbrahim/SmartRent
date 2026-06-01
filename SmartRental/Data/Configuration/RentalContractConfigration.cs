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
    public class RentalContractConfigration : IEntityTypeConfiguration<RentalContract>
    {
        public void Configure(EntityTypeBuilder<RentalContract> builder)
        {
           builder.HasOne(rc => rc.Apartment)
                    .WithMany(a => a.RentalContracts)
                    .HasForeignKey(rc => rc.ApartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(rc => rc.Tenant)
                    .WithMany(t => t.RentalContracts)
                    .HasForeignKey(rc => rc.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
             builder.Property(rc => rc.StartDate).IsRequired();
             builder.Property(rc => rc.EndDate).IsRequired();
             builder.Property(rc => rc.AnnualIncreasePercentage).IsRequired();
        }
    }
}
