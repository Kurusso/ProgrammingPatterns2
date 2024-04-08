namespace CoreApplication.Configurations
{
    public class RabbitMqConfigurations
    {
        public string HostName { get; set; }
        public string QueName { get; set; }
        public Uri Connection { get; set; }
        public string SecondQueName { get; set; }
    }
}
