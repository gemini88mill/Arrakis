using ProfiseeDevUtils.Init;
using System;
using System.Collections.Generic;

namespace ProfiseeDevUtilsTest.Mocks
{
    public class EnvironmentVariablesMock : EnvironmentVariables
    {
        private Dictionary<string, string> envVars = new Dictionary<string, string>();

        public EnvironmentVariablesMock(bool? quiet) : base(quiet)
        {
        }

        public override void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget envVarTarget)
        {
            this.envVars[variable] = value;
        }
        internal Dictionary<string, string> GetEnvironmentVariables()
        {
            return envVars;
        }
    }
}
