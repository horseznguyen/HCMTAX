using HCMTAX.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMTAX.API.Infrastructure
{
    public sealed partial class HCMTaxDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(@"User Id=hcmtax_read;Password=hcmtax_read2021;Data Source=192.168.0.103:1521/whiqlbl;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("iqlbl_owner");

            modelBuilder.Entity<PNNVDMKBNN>().ToView("PNN_V_DM_KBNN").HasNoKey();

            //modelBuilder.Entity<PTN>().ToView("iqlbl_owner.pnn_v_sono_hcmtax").HasNoKey();
            //modelBuilder.Entity<FullPTN>().ToView("iqlbl_owner.pnn_v_sono").HasNoKey();
            //modelBuilder.Entity<PNNVDMLOAITHUE>().ToView("iqlbl_owner.PNN_V_DM_LOAITHUE").HasNoKey();
            //modelBuilder.Entity<PNNVDMMTHU>().ToView("iqlbl_owner.PNN_V_DM_MTHU").HasNoKey();
        }
    }
}