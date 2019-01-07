using System;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;
using NLog.Targets;

[assembly: InternalsVisibleTo("NLog.Tests")]

namespace Scaffolding.NLog
{
    public static class NLogConfigExtensions
    {
	    public static void ConfigureFileTarget(this LoggingConfiguration config, string environmentName, ILogConfiguration logConfiguration = null)
	    {
		    FileTarget fileTarget = CreateFileTarget();
		    config.AddTarget("custom-web-file",fileTarget);

			LogLevel logLevel = LogLevel.Info;
			if (logConfiguration?.LogLevel != null)
				logLevel = logConfiguration?.LogLevel;
			else if(environmentName.Equals("Local", StringComparison.InvariantCultureIgnoreCase))
				logLevel = LogLevel.Debug;

			if (logConfiguration?.LoggingRules != null)
			{
				logConfiguration.LoggingRules
					.ForEach(l => config.LoggingRules
						.Add(new LoggingRule(l.LoggerNamePattern, l.Levels[0], fileTarget)));
			}
			else
			{
				var rule = new LoggingRule("Service*", logLevel, fileTarget);
				config.LoggingRules.Add(rule);
			}
		}

	    public static void ConfigureAllTarget(this LoggingConfiguration config)
	    {
			FileTarget fileTarget = CreateAllFileTarget();
		    config.AddTarget("all-logs", fileTarget);

		    LogLevel logLevel = LogLevel.Debug;

		    var rule = new LoggingRule("*", logLevel, fileTarget);
		    config.LoggingRules.Add(rule);
		}

	    private static FileTarget CreateAllFileTarget()
	    {
			var target = new FileTarget
			{
				FileName = "${var:logDirectory}/${var:appName}/all-log-${shortdate}.log",
				Layout =
					"${longdate}|${logger}|${uppercase:${level}}|  ${message} ${exception:format=toString}",
				ArchiveFileName = "${var:logDirectory}/${var:appName}/archives/all-log.{####}.log",
				ArchiveEvery = FileArchivePeriod.Day,
				ArchiveNumbering = ArchiveNumberingMode.Rolling,
				MaxArchiveFiles = 3,
				ConcurrentWrites = true,
				KeepFileOpen = false
			};

		    return target;
	    }

	    private static FileTarget CreateFileTarget()
	    {
			var target = new FileTarget
		    {
				FileName = "${var:logDirectory}/${var:appName}/log-${shortdate}.log",
			    Layout =
					"${longdate}|${logger}|${uppercase:${level}}|  ${message} ${exception:format=toString}",
			    ArchiveFileName = "${var:logDirectory}/${var:appName}/archives/log.{####}.log",
			    ArchiveEvery = FileArchivePeriod.Day,
			    ArchiveNumbering = ArchiveNumberingMode.Rolling,
			    MaxArchiveFiles = 10,
			    ConcurrentWrites = true,
			    KeepFileOpen = false
		    };

		    return target;
	    }
	}
}
