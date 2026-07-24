using Npgsql;

namespace DiscordApp.Server.DB
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Database connection string not found.");
        }

        public async Task InitializeAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            Console.WriteLine("PostgreSQL connected successfully.");
        }
    }
}   