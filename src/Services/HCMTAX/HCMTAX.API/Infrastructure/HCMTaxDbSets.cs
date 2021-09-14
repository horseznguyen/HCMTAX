using HCMTAX.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMTAX.API.Infrastructure
{
    public sealed partial class HCMTaxDbContext
    {
        //public DbSet<FullPTN> FullPTNS { get; set; }
        public DbSet<PNNVDMKBNN> PNNVDMKBNNS { get; set; }
        //public DbSet<PNNVDMLOAITHUE> PNNVDMLOAITHUES { get; set; }
        //public DbSet<PNNVDMMTHU> PNNVDMMTHUS { get; set; }
        //public DbSet<PTN> PTNS { get; set; }
    }
}