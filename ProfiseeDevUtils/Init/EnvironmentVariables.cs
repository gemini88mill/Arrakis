using Newtonsoft.Json;
using ProfiseeDevUtils.Infrastructure;
using Spectre.Console;

namespace ProfiseeDevUtils.Init
{
    public class EnvironmentVariables
    {
        private readonly bool quiet = false;
        private string TfsBaseDirPath = @"\DevOps";
        private Dictionary<string, string> envVars = new Dictionary<string, string>
        {
            // locals to populate full values
            { nameof(MaestroVersion), "22.2.0" },
            { nameof(TfsDrive), "C:"},
            { nameof(MaestroWebAppName), "Profisee"},
            { nameof(MaestroDb), "Profisee"},
            { nameof(TfsGetSource), "$/Products/"},
            { nameof(SqlDataPath), @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA"},
            { nameof(WebSiteName), "Default Web Site"},
            { nameof(SqlServer), "."},
            { nameof(UseWindowsAuthentication), "true"},
            { nameof(SqlUserName), ""},
            { nameof(SqlUserPassword), ""},
            { nameof(MaestroAppPoolName), "Profisee"},
            { nameof(MaestroAppPoolUserName), @"corp\svc_web"},
            { nameof(MaestroAppPoolUserPassword), "Profisee1"},
            { nameof(MaestroServicePort), "8003"},
            { nameof(MaestroServiceUserName), @"corp\svc_maestro"},
            { nameof(MaestroServiceUserPassword), "Profisee1"},
            { nameof(TfsUtil), @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\tf.exe"},
            { nameof(MsBuildUtil), @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe"},
            { nameof(MsTestUtil), @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\TestPlatform\VSTest.Console.exe"},
            { nameof(AttachmentRepositoryLocation), @"C:\FileRepository"},
            { nameof(AttachmentRepositoryUserName), @"corp\svc_web"},
            { nameof(AttachmentRepositoryUserPassword), "Profisee1"},
            { nameof(AttachmentRepositoryLogonType), "Interactive"},
            { nameof(UseHttps), "true"},
            { nameof(UseSeparateDatabases), "true"},
            { nameof(SetupTestCategory), "00-ApolloSetup-Small"},

            // automation settings
            { nameof(SqlEventingDatabaseName), "" },
            { nameof(SqlConnectorSqlServer), ""},
            { nameof(ServerURL), "net.tcp://127.0.0.1/Profisee"},
            { nameof(CrmSqlServer), ""},
            { nameof(CrmEventingDatabaseName), ""},
            { nameof(CrmDatabaseName), ""},
            { nameof(PathToFederation), @"C:\Program Files\Profisee\Master Data Maestro Integrator\4.0.1\IntegratorCLU.exe"},
            { nameof(RunAsUserName), ""},
            { nameof(ServerRESTVersion), "v1"},
            { nameof(ServerRESTUrl), "http://127.0.0.1/profisee/rest"},
        };

        public ILogger Logger { get; set; }

        public EnvironmentVariables(bool? quiet = null)
        {
            this.quiet = quiet ?? false;
            this.Logger = new Logger(this.quiet);
        }

        public void CreateCustomVarsFile()
        {
            var customVarsFilePath = this.getCustomVarsFilePath();
            if (File.Exists(customVarsFilePath))
            {
                this.Logger.Inform($"Custom vars file already exists at {customVarsFilePath}. Skipping creation.");
                return;
            }

            this.Logger.Inform($"Creating custom vars file at {customVarsFilePath}");
            Directory.CreateDirectory(this.getCustomVarsFileDirectory());
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
                        var task = singleTask ?? ctx.AddTask($"Set {envVar.Key} to {envVar.Value}", new ProgressTaskSettings
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
                this.Logger.Inform($"No custom vars file found at {file}");
                return new Dictionary<string, string>();
            }

            this.Logger.Inform($"Reading custom vars file at {file}");
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                vars = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            this.Logger.Inform($"Read {vars?.Count ?? 0} custom vars");
            return vars ?? new Dictionary<string, string>();
        }

        public void AddDerivedEnvVars()
        {
            var TfsBaseDir = $"{this.envVars[nameof(TfsDrive)]}{this.TfsBaseDirPath}";
            this.envVars[nameof(gitRepos)] = @$"{TfsBaseDir}\Repos";
            this.envVars[nameof(TfsSrc)] = @$"{this.envVars[nameof(gitRepos)]}\platform";
            this.envVars[nameof(ScriptsFolder)] = @$"{this.envVars[nameof(TfsSrc)]}\Scripts";
            this.envVars[nameof(BatchFileLocation)] = @$"{this.envVars[nameof(ScriptsFolder)]}\script_files";
            this.envVars[nameof(LicenseFile)] = @$"{this.envVars[nameof(BatchFileLocation)]}\Prof_2020r1_1Inst_3Nodes_OnlyProfiseeConnector_Production_@.corp.profisee.com.plic";
            //this.envVars[nameof(ScriptsDrive)] = this.envVars[nameof(TfsDrive)];
            this.envVars[nameof(TfsProto)] = @$"{TfsBaseDir}\Prototypes";
            this.envVars[nameof(TfsSdk)] = @$"{this.envVars[nameof(gitRepos)]}\sdk";
            this.envVars[nameof(AutomationFolder)] = @$"{this.envVars[nameof(TfsSrc)]}\Testing\Automation";
            this.envVars[nameof(UtilPath)] = @$"{this.envVars[nameof(TfsSrc)]}\Common\Utilities\bin\Debug";
            this.envVars[nameof(MaestroSvc)] = $"Profisee {this.envVars[nameof(MaestroVersion)]} ({this.envVars[nameof(MaestroWebAppName)]})";
            this.envVars[nameof(MaestroSnap)] = $"{this.envVars[nameof(MaestroDb)]}_Snapshot";
            this.envVars[nameof(TfsGetPlatformSource)] = $"{this.envVars[nameof(TfsGetSource)]}Platform";
            this.envVars[nameof(ServicesPublishPath)] = @$"{this.envVars[nameof(TfsSrc)]}\Server\Services";
            this.envVars[nameof(PathToUtilitiesExe)] = @$"{this.envVars[nameof(UtilPath)]}\Profisee.MasterDataMaestro.Utilities.exe";
        }

        public virtual string GetEnvVar(string variable)
        {
            return Environment.GetEnvironmentVariable(variable) ?? string.Empty;
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

        private string getCustomVarsFilePath()
        {
            return Path.Combine(this.getCustomVarsFileDirectory(), "customVars.json");
        }

        private string getCustomVarsFileDirectory()
        {
            string projectSourcePath = ProjectSourcePath.Value;
            return Path.Combine(projectSourcePath, "local");
        }

        public string AttachmentRepositoryLocation
        {
            get { return this.GetEnvVar(nameof(AttachmentRepositoryLocation)); }
        }
        public string AttachmentRepositoryLogonType
        {
            get { return this.GetEnvVar(nameof(AttachmentRepositoryLogonType)); }
        }

        public string AttachmentRepositoryUserName
        {
            get { return this.GetEnvVar(nameof(AttachmentRepositoryUserName)); }
        }

        public string AttachmentRepositoryUserPassword
        {
            get { return this.GetEnvVar(nameof(AttachmentRepositoryUserPassword)); }
        }

        public string MaestroWebAppName
        {
            get { return this.GetEnvVar(nameof(MaestroWebAppName)); }
        }

        public string MaestroAppPoolName
        {
            get { return this.GetEnvVar(nameof(MaestroAppPoolName)); }
        }

        public string MaestroAppPoolUserName
        {
            get { return this.GetEnvVar(nameof(MaestroAppPoolUserName)); }
        }

        public string MaestroAppPoolUserPassword
        {
            get { return this.GetEnvVar(nameof(MaestroAppPoolUserPassword)); }
        }

        public string MaestroDb
        {
            get { return this.GetEnvVar(nameof(MaestroDb)); }
        }

        public string MaestroServicePort
        {
            get { return this.GetEnvVar(nameof(MaestroServicePort)); }
        }

        public string MaestroServiceUserName
        {
            get { return this.GetEnvVar(nameof(MaestroServiceUserName)); }
        }

        public string MaestroServiceUserPassword
        {
            get { return this.GetEnvVar(nameof(MaestroServiceUserPassword)); }
        }

        public string MaestroVersion
        {
            get { return this.GetEnvVar(nameof(MaestroVersion)); }
        }

        public string MsBuildUtil
        {
            get { return this.GetEnvVar(nameof(MsBuildUtil)); }
        }

        public string MsTestUtil
        {
            get { return this.GetEnvVar(nameof(MsTestUtil)); }
        }

        public string SetupTestCategory
        {
            get { return this.GetEnvVar(nameof(SetupTestCategory)); }
        }

        public string SqlDataPath
        {
            get { return this.GetEnvVar(nameof(SqlDataPath)); }
        }

        public string SqlServer
        {
            get { return this.GetEnvVar(nameof(SqlServer)); }
        }

        public string SqlUserName
        {
            get { return this.GetEnvVar(nameof(SqlUserName)); }
        }

        public string SqlUserPassword
        {
            get { return this.GetEnvVar(nameof(SqlUserPassword)); }
        }

        public string TfsDrive
        {
            get { return this.GetEnvVar(nameof(TfsDrive)); }
        }

        public string TfsGetSource
        {
            get { return this.GetEnvVar(nameof(TfsGetSource)); }
        }

        public string TfsUtil
        {
            get { return this.GetEnvVar(nameof(TfsUtil)); }
        }

        public string UseHttps
        {
            get { return this.GetEnvVar(nameof(UseHttps)); }
        }

        public bool UseSeparateDatabases
        {
            get
            {
                bool retVal;
                bool.TryParse(this.GetEnvVar(nameof(UseSeparateDatabases)), out retVal);
                return retVal;
            }
        }

        public bool UseWindowsAuthentication
        {
            get 
            {
                bool retVal;
                bool.TryParse(this.GetEnvVar(nameof(UseWindowsAuthentication)), out retVal);
                return retVal;
            }
        }

        public string WebSiteName
        {
            get { return this.GetEnvVar(nameof(WebSiteName)); }
        }


        // automation settings
        public string CrmDatabaseName
        {
            get { return this.GetEnvVar(nameof(CrmDatabaseName)); }
        }

        public string CrmEventingDatabaseName
        {
            get { return this.GetEnvVar(nameof(CrmEventingDatabaseName)); }
        }

        public string CrmSqlServer
        {
            get { return this.GetEnvVar(nameof(CrmSqlServer)); }
        }

        public string PathToFederation
        {
            get { return this.GetEnvVar(nameof(PathToFederation)); }
        }

        public string RunAsUserName
        {
            get { return this.GetEnvVar(nameof(RunAsUserName)); }
        }

        public string ServerRESTUrl
        {
            get { return this.GetEnvVar(nameof(ServerRESTUrl)); }
        }

        public string ServerRESTVersion
        {
            get { return this.GetEnvVar(nameof(ServerRESTVersion)); }
        }

        public string ServerURL
        {
            get { return this.GetEnvVar(nameof(ServerURL)); }
        }

        public string SqlEventingDatabaseName
        {
            get { return this.GetEnvVar(nameof(SqlEventingDatabaseName)); }
        }

        public string SqlConnectorSqlServer
        {
            get { return this.GetEnvVar(nameof(SqlConnectorSqlServer)); }
        }


        // derived variables
        public string AutomationFolder
        {
            get { return this.GetEnvVar(nameof(AutomationFolder)); }
        }

        public string BatchFileLocation
        {
            get { return this.GetEnvVar(nameof(BatchFileLocation)); }
        }

        public string gitRepos
        {
            get { return this.GetEnvVar(nameof(gitRepos)); }
        }

        public string LicenseFile
        {
            get { return this.GetEnvVar(nameof(LicenseFile)); }
        }

        public string MaestroSnap
        {
            get { return this.GetEnvVar(nameof(MaestroSnap)); }
        }

        public string MaestroSvc
        {
            get { return this.GetEnvVar(nameof(MaestroSvc)); }
        }

        public string PathToUtilitiesExe
        {
            get { return this.GetEnvVar(nameof(PathToUtilitiesExe)); }
        }

        public string ScriptsFolder
        {
            get { return this.GetEnvVar(nameof(ScriptsFolder)); }
        }

        public string ServicesPublishPath
        {
            get { return this.GetEnvVar(nameof(ServicesPublishPath)); }
        }

        public string TfsGetPlatformSource
        {
            get { return this.GetEnvVar(nameof(TfsGetPlatformSource)); }
        }

        public string TfsProto
        {
            get { return this.GetEnvVar(nameof(TfsProto)); }
        }

        public string TfsSdk
        {
            get { return this.GetEnvVar(nameof(TfsSdk)); }
        }

        public string TfsSrc
        {
            get { return this.GetEnvVar(nameof(TfsSrc)); }
        }

        public string UtilPath
        {
            get { return this.GetEnvVar(nameof(UtilPath)); }
        }
    }
}
