using ProfiseeDevUtils.Init;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProfiseeDevUtilsTest.Mocks
{
    public class EnvironmentVariablesMock : EnvironmentVariables
    {
        private Dictionary<string, string> envVars = new Dictionary<string, string>();

        public EnvironmentVariablesMock(bool? quiet) : base(quiet)
        {
        }

        public override async Task SetEnvironmentVariable(string variable, string value, ProgressTask task)
        {
            await Task.Run(() =>
            {
                Environment.SetEnvironmentVariable(variable, value);
                this.envVars[variable] = value;
            });
        }
        internal Dictionary<string, string> GetEnvironmentVariables()
        {
            return envVars;
        }
    }
}
