using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ПП
{
    public class AppDbContext : DbContext
    {
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка профилей
            modelBuilder.Entity<Profile>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Profile>()
                .Property(p => p.CreatedDate)
                .HasColumnType("datetime2");
        }
    }
}
