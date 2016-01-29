using System.Data.SqlClient;
using System.Reflection;
using DbUp;
using Serilog;

namespace DataMigrations
{
    public class Migrator
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _masterConnectionString;

        public Migrator(string connectionString)
        {
            _connectionString = connectionString;

            // ReSharper disable once CollectionNeverQueried.Local
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);

            _databaseName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = "master";
            _masterConnectionString = connectionStringBuilder.ConnectionString;
        }

        public bool Boom(bool resetTheWorld)
        {
            if (resetTheWorld && DoesDatabaseExist())
            {
                DropDatabase();
            }

            if (!DoesDatabaseExist())
            {
                CreateDatabase();
            }

            return PerformMigrations();
        }

        private void DropDatabase()
        {
            Log.Information("Dropping {Database}", _databaseName);
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(
                        "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}];",
                        _databaseName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool DoesDatabaseExist()
        {
            Log.Information("Checking to see if {Database} exists", _databaseName);
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM [sys].[databases] WHERE [Name] = '{_databaseName}'";
                    var result = (int) command.ExecuteScalar();
                    return result > 0;
                }
            }
        }

        private void CreateDatabase()
        {
            Log.Information("Creating {Database}", _databaseName);
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE [{_databaseName}]";
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool PerformMigrations()
        {
            var result = DeployChanges.To.SqlDatabase(_connectionString)
                .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .LogTo(new SerilogUpgradeLog())
                .Build()
                .PerformUpgrade();

            return result.Successful;
        }
    }
}