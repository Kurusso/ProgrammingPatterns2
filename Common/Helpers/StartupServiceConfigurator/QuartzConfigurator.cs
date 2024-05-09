using Quartz;

namespace Common.Helpers.StartupServiceConfigurator
{
    public class QuartzConfigurator : CompositeConfigurator<IServiceCollectionQuartzConfigurator>
    {
        public override void PreConfigure(IServiceCollectionQuartzConfigurator q)
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
        }
    }
}
