using System.IO;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using NLog;
using NLog.Targets;
using Xunit;

namespace Scaffolding.NLog.Tests
{
    public class NLogConfigTests
    {
	    private readonly Mock<IHostingEnvironment> _mockEnvironment;
	    private readonly Mock<ILogConfiguration> _mockLogConfiguration;

	    private const string CUSTOM_APP_NAME = "CustomApp";
	    private const string ENVN_APP_NAME = "EnvnApp";
		private const string LOG_DIR = "logs";
	    private const string LOG_DIR_LOCAL = "localLogs";

	    public NLogConfigTests()
	    {
			_mockEnvironment = new Mock<IHostingEnvironment>();
			_mockLogConfiguration = new Mock<ILogConfiguration>();
		    _mockEnvironment.SetupGet(env => env.ApplicationName).Returns(ENVN_APP_NAME);
		    _mockLogConfiguration.SetupGet(config => config.LogDirectory).Returns(LOG_DIR);
		    _mockLogConfiguration.SetupGet(config => config.LogDirectoryLocal).Returns(LOG_DIR_LOCAL);
		}

        [Fact]
        public void CreateConfiguration_Returns_Default_LoggingConfiguration()
        {
	        _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Local");

	        var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object);

	        loggingConfiguration.Should().NotBe(null);
	        loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
	        loggingConfiguration.LoggingRules.Should().NotBeEmpty();
	        loggingConfiguration.Variables.Should().NotBeEmpty();
	        loggingConfiguration.Variables.Should().ContainKey("appName");
	        loggingConfiguration.Variables.Should().ContainKey("logDirectory");
	        loggingConfiguration.Variables["appName"].Text.Should().Be(ENVN_APP_NAME);
			loggingConfiguration.ConfiguredNamedTargets.Count.Should().BeGreaterThan(1);
	        loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
	        loggingConfiguration.ConfiguredNamedTargets[1].Should().BeOfType(typeof(FileTarget));
		}

		[Fact]
	    public void CreateConfiguration_Returns_Custom_LoggingConfiguration()
	    {
		    _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Local");
		    _mockLogConfiguration.SetupGet(config => config.AppName).Returns(CUSTOM_APP_NAME);
		    _mockLogConfiguration.SetupGet(config => config.EnableWebLogs).Returns(true);
		    _mockLogConfiguration.SetupGet(config => config.EnableAllLogs).Returns(true);

		    var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object, _mockLogConfiguration.Object);

		    loggingConfiguration.Should().NotBe(null);
		    loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().ContainKey("appName");
		    loggingConfiguration.Variables.Should().ContainKey("logDirectory");
		    loggingConfiguration.Variables["appName"].Text.Should().Be(CUSTOM_APP_NAME);
		    loggingConfiguration.ConfiguredNamedTargets.Count.Should().BeGreaterThan(1);
		    loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
		    loggingConfiguration.ConfiguredNamedTargets[1].Should().BeOfType(typeof(FileTarget));
	    }

	    [Fact]
	    public void CreateConfiguration_Returns_No_All_Logs_Target_LoggingConfiguration()
	    {
		    _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Local");
		    _mockLogConfiguration.SetupGet(config => config.AppName).Returns(CUSTOM_APP_NAME);
		    _mockLogConfiguration.SetupGet(config => config.EnableWebLogs).Returns(true);
		    _mockLogConfiguration.SetupGet(config => config.EnableAllLogs).Returns(false);

		    var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object, _mockLogConfiguration.Object);

		    loggingConfiguration.Should().NotBe(null);
		    loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().ContainKey("appName");
		    loggingConfiguration.Variables.Should().ContainKey("logDirectory");
		    loggingConfiguration.Variables["appName"].Text.Should().Be(CUSTOM_APP_NAME);
		    loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
		    loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
	    }

	    [Fact]
	    public void CreateConfiguration_Returns_No_FileTarget_LoggingConfiguration()
	    {
		    _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Local");
		    _mockLogConfiguration.SetupGet(config => config.AppName).Returns(CUSTOM_APP_NAME);
		    _mockLogConfiguration.SetupGet(config => config.EnableWebLogs).Returns(false);
		    _mockLogConfiguration.SetupGet(config => config.EnableAllLogs).Returns(true);

		    var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object, _mockLogConfiguration.Object);

		    loggingConfiguration.Should().NotBe(null);
		    loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().ContainKey("appName");
		    loggingConfiguration.Variables.Should().ContainKey("logDirectory");
		    loggingConfiguration.Variables["appName"].Text.Should().Be(CUSTOM_APP_NAME);
		    loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
		    loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
	    }

	    [Fact]
	    public void CreateConfiguration_Does_Not_Create_All_Logs_Target_In_Test_Envn()
	    {
		    _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Test");

		    var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object);

		    loggingConfiguration.Should().NotBe(null);
		    loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().ContainKey("appName");
		    loggingConfiguration.Variables.Should().ContainKey("logDirectory");
		    loggingConfiguration.Variables["appName"].Text.Should().Be(ENVN_APP_NAME);
		    loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
		    loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
	    }

	    [Fact]
	    public void CreateConfiguration_Using_Appsettings()
	    {
		    _mockEnvironment.SetupGet(env => env.EnvironmentName).Returns("Local");
		    _mockLogConfiguration.SetupGet(config => config.AppName).Returns(CUSTOM_APP_NAME);
		    _mockLogConfiguration.SetupGet(config => config.EnableWebLogs).Returns(false);
		    _mockLogConfiguration.SetupGet(config => config.EnableAllLogs).Returns(true);

		    var builder = new ConfigurationBuilder()
			    .SetBasePath(Directory.GetCurrentDirectory())
			    .AddJsonFile("appsettings.json", optional: false);

		    var testConfig = builder.Build();
		    var nlogConfig = testConfig.GetSection("Logging:NLog");

			var loggingConfiguration = NLogConfig.CreateConfiguration(_mockEnvironment.Object, nlogConfig);

		    loggingConfiguration.Should().NotBe(null);
		    loggingConfiguration.ConfiguredNamedTargets.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Should().NotBeEmpty();
		    loggingConfiguration.LoggingRules.Count.Should().Be(5);
		    loggingConfiguration.LoggingRules[0].LoggerNamePattern.Should().Be("*");
		    loggingConfiguration.LoggingRules[0].Levels[0].Should().Be(LogLevel.Debug);
		    loggingConfiguration.LoggingRules[1].LoggerNamePattern.Should().Be("*Controller");
		    loggingConfiguration.LoggingRules[1].Levels[0].Should().Be(LogLevel.Trace);
		    loggingConfiguration.LoggingRules[2].LoggerNamePattern.Should().Be("Default");
		    loggingConfiguration.LoggingRules[2].Levels[0].Should().Be(LogLevel.Debug);
		    loggingConfiguration.LoggingRules[3].LoggerNamePattern.Should().Be("ErrorHandlingMiddleware");
		    loggingConfiguration.LoggingRules[3].Levels[0].Should().Be(LogLevel.Info);
		    loggingConfiguration.LoggingRules[4].LoggerNamePattern.Should().Be("Service*");
		    loggingConfiguration.LoggingRules[4].Levels[0].Should().Be(LogLevel.Error);
		    loggingConfiguration.Variables.Should().NotBeEmpty();
		    loggingConfiguration.Variables.Should().ContainKey("appName");
		    loggingConfiguration.Variables.Should().ContainKey("logDirectory");
		    loggingConfiguration.Variables["appName"].Text.Should().Be(ENVN_APP_NAME);
		    loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(2);
		    loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
	    }

	    [Fact]
	    public void Build_Logging_Rule()
	    {
		    var rule = NLogConfig.BuildLoggingRule("*Service", "Trace", LogLevel.Info);
		    rule.LoggerNamePattern.Should().Be("*Service");
		    rule.Levels[0].Should().Be(LogLevel.Trace);

		    var rule2 = NLogConfig.BuildLoggingRule("*Service", "Error", LogLevel.Info);
		    rule2.LoggerNamePattern.Should().Be("*Service");
		    rule2.Levels[0].Should().Be(LogLevel.Error);

		    var rule3 = NLogConfig.BuildLoggingRule("*Service", "Garbage", LogLevel.Info);
		    rule3.LoggerNamePattern.Should().Be("*Service");
		    rule3.Levels[0].Should().Be(LogLevel.Info);
	    }
	}
}
