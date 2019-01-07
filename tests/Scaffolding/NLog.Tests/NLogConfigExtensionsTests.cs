using System.Collections.Generic;
using FluentAssertions;
using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit;

namespace Scaffolding.NLog.Tests
{
	public class NLogConfigExtensionsTests
	{
		[Fact]
		public void ConfigureFileTarget_Configures_File_Target()
		{
			var loggingConfiguration = new LoggingConfiguration();

			loggingConfiguration.ConfigureFileTarget("Local");

			loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
			loggingConfiguration.LoggingRules.Count.Should().Be(1);
			loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));

			FileTarget fileTarget = (FileTarget)loggingConfiguration.ConfiguredNamedTargets[0];
			fileTarget.Layout.Should().NotBeNull();
			fileTarget.FileName.Should().NotBeNull();
		}

		[Fact]
		public void ConfigureAllTarget_Configures_All_Logs_Target()
		{
			var loggingConfiguration = new LoggingConfiguration();

			loggingConfiguration.ConfigureAllTarget();

			loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
			loggingConfiguration.LoggingRules.Count.Should().Be(1);
			loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));

			FileTarget allLogsTarget = (FileTarget)loggingConfiguration.ConfiguredNamedTargets[0];

			allLogsTarget.MaxArchiveFiles.Should().Be(3);
		}

		[Fact]
		public void ConfigureFileTarget_Does_Not_Set_Debug_LogLevel_For_Test_Environment()
		{
			var loggingConfiguration = new LoggingConfiguration();

			loggingConfiguration.ConfigureFileTarget("Test");

			loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
			loggingConfiguration.LoggingRules.Count.Should().Be(1);
			loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));
			loggingConfiguration.LoggingRules[0].Levels.Should().NotContain(LogLevel.Debug);
		}

		[Fact]
		public void ConfigureFileTarget_Configures_Log_Archival()
		{
			var loggingConfiguration = new LoggingConfiguration();

			loggingConfiguration.ConfigureFileTarget("Local");

			loggingConfiguration.ConfiguredNamedTargets.Count.Should().Be(1);
			loggingConfiguration.LoggingRules.Count.Should().Be(1);
			loggingConfiguration.LoggingRules[0].Levels[0].Should().Be(LogLevel.Debug);
			loggingConfiguration.ConfiguredNamedTargets[0].Should().BeOfType(typeof(FileTarget));

			FileTarget fileTarget = (FileTarget) loggingConfiguration.ConfiguredNamedTargets[0];

			fileTarget.ArchiveFileName.Should().NotBeNull();
			fileTarget.ArchiveNumbering.Should().Be(ArchiveNumberingMode.Rolling);
			fileTarget.ArchiveEvery.Should().Be(FileArchivePeriod.Day);
			fileTarget.MaxArchiveFiles.Should().BeGreaterThan(1);
		}
		[Fact]
		public void ConfigureFileTarget_Configures_With_CustomRules()
		{
			var loggingConfiguration = new LoggingConfiguration();
			var loggingRules = new List<LoggingRule>()
			{
				new LoggingRule("Service*", LogLevel.Debug, null),
				new LoggingRule("*Controller", LogLevel.Fatal, null),
				new LoggingRule("ErrorHandlingMiddleware", LogLevel.Trace, null)
			};

			loggingConfiguration.ConfigureFileTarget("Local", new NLogConfiguration()
			{
				LogLevel = LogLevel.Fatal,
				LoggingRules = loggingRules
			});

			loggingConfiguration.LoggingRules.Count.Should().Be(3);
			loggingConfiguration.LoggingRules[0].Levels[0].Should().Be(LogLevel.Debug);
			loggingConfiguration.LoggingRules[0].LoggerNamePattern.Should().Be("Service*");
			loggingConfiguration.LoggingRules[1].Levels[0].Should().Be(LogLevel.Fatal);
			loggingConfiguration.LoggingRules[1].LoggerNamePattern.Should().Be("*Controller");
			loggingConfiguration.LoggingRules[2].Levels[0].Should().Be(LogLevel.Trace);
			loggingConfiguration.LoggingRules[2].LoggerNamePattern.Should().Be("ErrorHandlingMiddleware");
		}

		[Fact]
		public void ConfigureFileTarget_Using_Coded_Rules()
		{
			var loggingConfiguration = new LoggingConfiguration();
			var loggingRules = new List<LoggingRule>()
			{
				new LoggingRule("Service*", LogLevel.Debug, null),
				new LoggingRule("*Controller", LogLevel.Fatal, null),
				new LoggingRule("ErrorHandlingMiddleware", LogLevel.Trace, null)
			};

			loggingConfiguration.ConfigureFileTarget("Local", new NLogConfiguration()
			{
				LogLevel = LogLevel.Fatal,
				LoggingRules = loggingRules
			});

			loggingConfiguration.LoggingRules.Count.Should().Be(3);
			loggingConfiguration.LoggingRules[0].Levels[0].Should().Be(LogLevel.Debug);
			loggingConfiguration.LoggingRules[0].LoggerNamePattern.Should().Be("Service*");
			loggingConfiguration.LoggingRules[1].Levels[0].Should().Be(LogLevel.Fatal);
			loggingConfiguration.LoggingRules[1].LoggerNamePattern.Should().Be("*Controller");
			loggingConfiguration.LoggingRules[2].Levels[0].Should().Be(LogLevel.Trace);
			loggingConfiguration.LoggingRules[2].LoggerNamePattern.Should().Be("ErrorHandlingMiddleware");
		}

	}
}