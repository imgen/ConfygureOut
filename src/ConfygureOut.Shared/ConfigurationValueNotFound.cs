namespace ConfygureOut
{
    public sealed class ConfigurationValueNotFound
    {
        private static volatile ConfigurationValueNotFound _instance;
        private static readonly object SyncRoot = new object();

        private ConfigurationValueNotFound() { }

        public static ConfigurationValueNotFound Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ConfigurationValueNotFound();
                    }
                }

                return _instance;
            }
        }
    }
}
