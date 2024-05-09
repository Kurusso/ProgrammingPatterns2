using Quartz;

namespace Common.Extensions
{
    public class QuartzConfigurator
    {
        private readonly List<Action<IServiceCollectionQuartzConfigurator>> _configurators = new();

        public QuartzConfigurator Append(Action<IServiceCollectionQuartzConfigurator> configAction)
        {
            _configurators.Add(configAction);
            return this;
        }
        public void Configure(IServiceCollectionQuartzConfigurator q)
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            foreach (var configurator in _configurators)
            {
                configurator.Invoke(q);
            }
            //return q;
        }
    }
}
