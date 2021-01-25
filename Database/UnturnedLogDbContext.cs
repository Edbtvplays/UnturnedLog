using System;
using Microsoft.EntityFrameworkCore;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using OpenMod.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Edbtvplays.UnturnedLog.Unturned.Database
{
    // This Context is for Static Data, as we have two types of database being used. A time series one and a normal database two of these declerations is neccecary to make.


    public class UnturnedLogDbContext : OpenModDbContext<UnturnedLogDbContext>
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<PlayerData> Players { get; set; }

        public UnturnedLogDbContext(DbContextOptions<UnturnedLogDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }

        //public UnturnedLogStaticDbContext(DbContextOptions<UnturnedLogStaticDbContext> options,
        //    IServiceProvider serviceProvider) : base(options, serviceProvider) // Creates new query with options using the service provider. 
            
        //{
        //    m_ServiceProvider = serviceProvider;
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionStringName = GetConnectionStringName();
        //    var connectionStringAccessor = m_ServiceProvider.GetRequiredService<IConnectionStringAccessor>();
        //    var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);
        //    optionsBuilder.UseNpgsql(connectionString);
        //}
}



