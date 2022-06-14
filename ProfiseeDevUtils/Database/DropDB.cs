using Microsoft.Data.SqlClient;
using ProfiseeDevUtils.Infrastructure;
using ProfiseeDevUtils.Init;

namespace ProfiseeDevUtils.Database
{
    internal class DropDB
    {
        private Infrastructure.Database database;
        private string sqlServer;
        private string dbName;
        private bool useSeparateDbs;
        public ILogger Logger { get; set; }

        public DropDB(bool quiet, string? sqlServer, string? dbName)
        {
            var envVars = new EnvironmentVariables(quiet);
            this.sqlServer = sqlServer ?? envVars.SqlServer;
            this.dbName = dbName ?? envVars.MaestroDb;
            this.useSeparateDbs = envVars.UseSeparateDatabases;
            this.Logger = new Logger(quiet);
            this.database = new Infrastructure.Database(
                quiet,
                this.sqlServer,
                envVars.UseWindowsAuthentication,
                envVars.SqlUserName,
                envVars.SqlUserPassword);
        }

        public static void Act(string? sqlServer, string? dbName, bool? quiet)
        {
            var isQuiet = quiet ?? false;
            var dropDb = new DropDB(isQuiet, sqlServer, dbName);
            dropDb.process();
        }

        private void process()
        {
            this.database.Drop(this.dbName);
            if (this.useSeparateDbs)
            {
                this.database.Drop($"{this.dbName}_Messaging");
                this.database.Drop($"{this.dbName}_Logging");
            }
        }
    }
}
