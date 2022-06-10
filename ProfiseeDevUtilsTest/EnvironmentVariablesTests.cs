using NUnit.Framework;
using ProfiseeDevUtilsTest.Mocks;
using System;

namespace ProfiseeDevUtilsTest
{
    public class EnvironmentVariablesTests
    {
        private EnvironmentVariablesMock environmentVariablesMock = new EnvironmentVariablesMock(false);

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EnvironmentVariables_Set_SetsEnvVars()
        {            
            this.environmentVariablesMock.SetAllAsync().Wait();

            var envVars = this.environmentVariablesMock.GetEnvironmentVariables();
            foreach (var envVar in envVars)
            {
                var myEnvVar = Environment.GetEnvironmentVariable(envVar.Key, EnvironmentVariableTarget.User);
                Assert.AreEqual(myEnvVar, envVar.Value, $"Expected {envVar.Key} to be {myEnvVar} but instead  was {envVar.Value}");
            }
        }
    }
}