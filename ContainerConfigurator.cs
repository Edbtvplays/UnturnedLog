using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using Edbtvplays.UnturnedLog.Unturned.Database;

namespace Edbtvplays.UnturnedLog.Unturned
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<UnturnedLogStaticDbContext>();
        }
    }
}