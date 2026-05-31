namespace NguyenBinhAn_A02_Data
{
    /// <summary>
    /// Singleton pattern: manages the database connection string configuration.
    /// Only one instance exists throughout the application lifetime.
    /// </summary>
    public sealed class FUNewsManagementDbContextFactory
    {
        private static FUNewsManagementDbContextFactory? _instance;
        private static readonly object _lock = new object();

        private string _connectionString = string.Empty;

        // Private constructor prevents external instantiation
        private FUNewsManagementDbContextFactory() { }

        /// <summary>
        /// Gets the single instance of FUNewsManagementDbContextFactory (Singleton pattern).
        /// Thread-safe using double-checked locking.
        /// </summary>
        public static FUNewsManagementDbContextFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FUNewsManagementDbContextFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes the factory with the connection string from configuration.
        /// Should be called once at application startup.
        /// </summary>
        public void Initialize(string connectionString)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = connectionString;
            }
        }

        /// <summary>
        /// Gets the configured connection string.
        /// </summary>
        public string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("FUNewsManagementDbContextFactory has not been initialized. Call Initialize() first.");
            return _connectionString;
        }
    }
}
