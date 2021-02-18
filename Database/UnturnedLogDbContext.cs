using System;
using Microsoft.EntityFrameworkCore;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using OpenMod.EntityFrameworkCore;

namespace Edbtvplays.UnturnedLog.Unturned.Database
{
    public class UnturnedLogDbContext : OpenModDbContext<UnturnedLogDbContext>
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<PlayerData> Players { get; set; }

        public UnturnedLogDbContext(DbContextOptions<UnturnedLogDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}



