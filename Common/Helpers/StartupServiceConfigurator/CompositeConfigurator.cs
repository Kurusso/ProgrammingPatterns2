namespace Common.Helpers.StartupServiceConfigurator
{
    public class CompositeConfigurator<TServiceCollectionConfigurator>
    {
        protected readonly List<Action<TServiceCollectionConfigurator>> _configurators = new();
        public CompositeConfigurator<TServiceCollectionConfigurator> Append(Action<TServiceCollectionConfigurator> configAction)
        {
            _configurators.Add(configAction);
            return this;
        }
        public virtual void PreConfigure(TServiceCollectionConfigurator q)
        {

        }

        public void Configure(TServiceCollectionConfigurator q)
        {
            PreConfigure(q);

            foreach (var configurator in _configurators)
            {
                configurator.Invoke(q);
            }
            //return q;
        }
    }
}
