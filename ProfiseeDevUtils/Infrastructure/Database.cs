using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfiseeDevUtils.Infrastructure
{
    internal class Database
    {
        private string sqlServer;
        private string connectionString;
        public ILogger Logger { get; set; }

        public Database(
            bool quiet,
            string sqlServer,
            bool integratedSecurity = true,
            string userName = "",
            string password = "",
            string dbName = ""
            )
        {
            this.Logger = new Logger(quiet);
            this.sqlServer = sqlServer;
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = sqlServer;
            builder.PersistSecurityInfo = true;
            builder.Pooling = true;
            builder.Encrypt = false;
            builder.IntegratedSecurity = integratedSecurity;
            if (!integratedSecurity)
            {
                builder.UserID = userName;
                builder.Password = password;
            }

            if (!string.IsNullOrWhiteSpace(dbName))
            {
                builder.InitialCatalog = dbName;
            }

            this.connectionString = builder.ConnectionString;
        }

        public void Drop(string dbName)
        {
            string commandSql = $@"
            IF EXISTS(SELECT * FROM sys.databases WHERE name = '{dbName}_Snapshot' AND source_database_id IS NOT NULL) DROP DATABASE {dbName}_Snapshot;
            IF EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') 
            BEGIN
                ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
                DROP DATABASE [{dbName}]
            END
                     ";
            this.Logger.Inform($"Dropping database {dbName} from {this.sqlServer}");
            executeSqlCommand(commandSql);
            this.Logger.Inform($"Database {dbName} deleted");
        }

        private int executeSqlCommand(string commandSql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandSql, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
