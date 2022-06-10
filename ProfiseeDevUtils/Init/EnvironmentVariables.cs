using Newtonsoft.Json;
using ProfiseeDevUtils.Infrastructure;
using Spectre.Console;
using VarName = ProfiseeDevUtils.Init.EnvironmentVariableNames;

namespace ProfiseeDevUtils.Init
{
    public class EnvironmentVariables
    {
        private readonly bool quiet = false;
        private string TfsBaseDirPath = @"\DevOps";
        private Dictionary<string, string> envVars = new Dictionary<string, string>
        {
            // locals to populate full values
            { VarName.MaestroVersion, "22.2.0" },
            { VarName.TfsDrive, "C:"},
            { VarName.MaestroWebAppName, "Profisee"},
            { VarName.MaestroDb, "Profisee"},
            { VarName.TfsGetSource, "$/Products/"},
            { VarName.SqlDataPath, @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA"},
            { VarName.WebSiteName, "Default Web Site"},
            { VarName.SqlServer, "."},
            { VarName.UseWindowsAuthentication, "true"},
            { VarName.SqlUserName, ""},
            { VarName.SqlUserPassword, ""},
            { VarName.MaestroAppPoolName, "Profisee"},
            { VarName.MaestroAppPoolUserName, @"corp\svc_web"},
            { VarName.MaestroAppPoolUserPassword, "Profisee1"},
            { VarName.MaestroServicePort, "8003"},
            { VarName.MaestroServiceUserName, @"corp\svc_maestro"},
            { VarName.MaestroServiceUserPassword, "Profisee1"},
            { VarName.TfsUtil, @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\tf.exe"},
            { VarName.MsBuildUtil, @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe"},
            { VarName.MsTestUtil, @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\Extensions\TestPlatform\VSTest.Console.exe"},
            { VarName.AttachmentRepositoryLocation, @"C:\FileRepository"},
            { VarName.AttachmentRepositoryUserName, @"corp\svc_web"},
            { VarName.AttachmentRepositoryUserPassword, "Profisee1"},
            { VarName.AttachmentRepositoryLogonType, "Interactive"},
            { VarName.UseHttps, "true"},
            { VarName.UseSeparateDatabases, "true"},
            { VarName.SetupTestCategory, "00-ApolloSetup-Small"},

            // automation settings
            { VarName.SqlEventingDatabaseName, "" },
            { VarName.SqlConnectorSqlServer, ""},
            { VarName.ServerURL, "net.tcp://127.0.0.1/Profisee"},
            { VarName.CrmSqlServer, ""},
            { VarName.CrmEventingDatabaseName, ""},
            { VarName.CrmDatabaseName, ""},
            { VarName.PathToFederation, @"C:\Program Files\Profisee\Master Data Maestro Integrator\4.0.1\IntegratorCLU.exe"},
            { VarName.RunAsUserName, ""},
            { VarName.ServerRESTVersion, "v1"},
            { VarName.ServerRESTUrl, "http://127.0.0.1/profisee/rest"},
        };

        public ILogger Logger { get; set; } = new Logger();

        public EnvironmentVariables(bool? quiet = null)
        {
            this.quiet = quiet ?? false;
        }

        public void CreateCustomVarsFile()
        {
            var customVarsFilePath = this.getCustomVarsFilePath();
            if (File.Exists(customVarsFilePath))
            {
                this.log($"Custom vars file already exists at {customVarsFilePath}. Skipping creation.");
                return;
            }

            this.log($"Creating custom vars file at {customVarsFilePath}");
            var machineVars = new { ServerRESTUrl = $"https://{Environment.MachineName}.corp.profisee.com/Profisee/rest/" };
            string json = JsonConvert.SerializeObject(machineVars, Formatting.Indented);
            File.WriteAllText(customVarsFilePath, json);
        }

        /// <summary>
        /// Sets the environment variables
        /// </summary>
        public async Task SetAllAsync()
        {
            var customVarsFilePath = this.getCustomVarsFilePath();
            var customVars = this.ParseCustomVars(customVarsFilePath);

            foreach(var customVar in customVars)
            {
                this.envVars[customVar.Key] = customVar.Value;
            }

            this.AddDerivedEnvVars();
            foreach(var customVar in customVars)
            {
                this.envVars[customVar.Key] = customVar.Value;
            }

            await AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    var tasks = new List<ProgressTask>(this.envVars.Count);
                    var singleTask = this.quiet ? ctx.AddTask("Set env vars", new ProgressTaskSettings
                    {
                        AutoStart = true,
                        MaxValue = this.envVars.Count * 2
                    }) : null;
                    await Task.WhenAll(this.envVars.Select(async (envVar, i) =>
                    {
                        var task = this.quiet ? singleTask : ctx.AddTask($"Set {envVar.Key} to {envVar.Value}", new ProgressTaskSettings
                        {
                            AutoStart = true,
                            MaxValue = 2
                        });
                        await this.SetEnvironmentVariable(envVar.Key, envVar.Value, task);
                    }));
                    
                });
        }

        public Dictionary<string, string> ParseCustomVars(string file)
        {
            Dictionary<string, string>? vars;
            if (!File.Exists(file))
            {
                this.log($"No custom vars file found at {file}");
                return new Dictionary<string, string>();
            }

            this.log($"Reading custom vars file at {file}");
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                vars = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            this.log($"Read {vars?.Count ?? 0} custom vars");
            return vars ?? new Dictionary<string, string>();
        }

        public void AddDerivedEnvVars()
        {
            var TfsBaseDir = $"{this.envVars[VarName.TfsDrive]}{this.TfsBaseDirPath}";
            this.envVars[VarName.gitRepos] = @$"{TfsBaseDir}\Repos";
            this.envVars[VarName.TfsSrc] = @$"{this.envVars[VarName.gitRepos]}\platform";
            this.envVars[VarName.ScriptsFolder] = @$"{this.envVars[VarName.TfsSrc]}\Scripts";
            this.envVars[VarName.BatchFileLocation] = @$"{this.envVars[VarName.ScriptsFolder]}\script_files";
            this.envVars[VarName.LicenseFile] = @$"{this.envVars[VarName.BatchFileLocation]}\Prof_2020r1_1Inst_3Nodes_OnlyProfiseeConnector_Production_@.corp.profisee.com.plic";
            //this.envVars[VarName.ScriptsDrive] = this.envVars[VarName.TfsDrive];
            this.envVars[VarName.TfsProto] = @$"{TfsBaseDir}\Prototypes";
            this.envVars[VarName.TfsSdk] = @$"{this.envVars[VarName.gitRepos]}\sdk";
            this.envVars[VarName.AutomationFolder] = @$"{this.envVars[VarName.TfsSrc]}\Testing\Automation";
            this.envVars[VarName.UtilPath] = @$"{this.envVars[VarName.TfsSrc]}\Common\Utilities\bin\Debug";
            this.envVars[VarName.MaestroSvc] = $"Profisee {this.envVars[VarName.MaestroVersion]} ({this.envVars[VarName.MaestroWebAppName]})";
            this.envVars[VarName.MaestroSnap] = $"{this.envVars[VarName.MaestroDb]}_Snapshot";
            this.envVars[VarName.TfsGetPlatformSource] = $"{this.envVars[VarName.TfsGetSource]}Platform";
            this.envVars[VarName.ServicesPublishPath] = @$"{this.envVars[VarName.TfsSrc]}\Server\Services";
            this.envVars[VarName.PathToUtilitiesExe] = @$"{this.envVars[VarName.UtilPath]}\Profisee.MasterDataMaestro.Utilities.exe";
        }

        public virtual string? GetEnvVar(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public virtual async Task SetEnvironmentVariable(string variable, string value, ProgressTask task)
        {
            await Task.Run(() =>
            {
                Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
                task.Increment(1);
                Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
                task.Increment(1);
            });
        }

        private void log(string message)
        {
            if (this.quiet) return;

            this.Logger.WriteLine(message);
        }

        private string getCustomVarsFilePath()
        {
            string projectSourcePath = ProjectSourcePath.Value;
            return Path.Combine(projectSourcePath, "local", "customVars.json");
        }
    }
}
