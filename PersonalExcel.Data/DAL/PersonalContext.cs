using Microsoft.EntityFrameworkCore;
using PersonalExeclReader.Data.Entity;

namespace PersonalExeclReader.Data.DAL
{
    /// <summary>
    /// Person Database Context and Table
    /// </summary>
    public partial class PersonalContext : DbContext
    {

        public PersonalContext(DbContextOptions<PersonalContext> options) : base(options)
        {
        }

        public virtual DbSet<Personal> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=PersonalExcel;Trusted_Connection=True;");
            }
        }
    }
}
