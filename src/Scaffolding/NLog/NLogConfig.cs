using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;
using NLog.Config;

namespace Scaffolding.NLog
{
    public static class NLogConfig
    {
		/// <summary>
		/// Creates LoggingConfiguration for NLog based on hosting environment
		/// </summary>
		/// <param name="env">Current hosting environment of the application</param>
		/// <param name="logConfiguration">[optional] Custom configuration for the application</param>
	    public static LoggingConfiguration CreateConfiguration(IHostingEnvironment env, ILogConfiguration logConfiguration = null)
	    {
			NLogConfiguration nLogConfig = SetUpNLogConfig(env, logConfiguration);

		    var loggingConfiguration = new LoggingConfiguration();

			string basePath = PlatformServices.Default.Application.ApplicationBasePath;
		    string logDirectory = $"{basePath}/{GetLogDirectory(env, nLogConfig)}";

			loggingConfiguration.Variables["appName"] = nLogConfig.AppName;
			loggingConfiguration.Variables["logDirectory"] = logDirectory;

			if(nLogConfig.EnableAllLogs)
				loggingConfiguration.ConfigureAllTarget();

			if(nLogConfig.EnableWebLogs)
				loggingConfiguration.ConfigureFileTarget(GetEnvironmentName(env), logConfiguration);

			return loggingConfiguration;
		}

	    public static LoggingConfiguration CreateConfiguration(IHostingEnvironment env, IConfigurationSection configSection)
	    {
			var defaultLogLevel = configSection.GetValue<string>("LogLevel:Default");
			Console.WriteLine($"NLOG Default Logging Level = {defaultLogLevel} ");
			NLogConfiguration nLogConfig = SetUpNLogConfig(env, null);
			nLogConfig.LogLevel = LogLevel.FromString(defaultLogLevel)?? nLogConfig.LogLevel;

		    var logLevelSection = configSection.GetSection("LogLevel");
		    nLogConfig.LoggingRules = logLevelSection.GetChildren()
				.Select(n => BuildLoggingRule(n.Key, n.Value, nLogConfig.LogLevel)).ToList();
			return CreateConfiguration(env, nLogConfig);
	    }
	    public static LoggingRule BuildLoggingRule(string entry, string level, LogLevel defaultLogLevel)
	    {
			var logLevel = defaultLogLevel;
			try
			{
				logLevel = LogLevel.FromString(level) ?? defaultLogLevel;
			}
			catch {}
			Console.WriteLine($"LogRule: {entry}, {logLevel}");
		    TrackDebugEnabled(entry, logLevel);
			return new LoggingRule(entry, logLevel, null);
	    }

		private static string GetLogDirectory(IHostingEnvironment env, ILogConfiguration nLogConfig)
	    {
		    var environment = GetEnvironmentName(env);

		    string logDirectory = environment.Equals("Local", StringComparison.InvariantCultureIgnoreCase)
			    ? nLogConfig.LogDirectoryLocal
			    : nLogConfig.LogDirectory;
		    return logDirectory;
	    }

	    private static NLogConfiguration SetUpNLogConfig(IHostingEnvironment env, ILogConfiguration logConfiguration)
	    {
		    return logConfiguration == null 
			    ? DefaultNLogConfiguration(env) 
			    : CustomNLogConfiguration(logConfiguration, env);
	    }

	    private static NLogConfiguration CustomNLogConfiguration(ILogConfiguration logConfiguration, IHostingEnvironment env)
	    {
		    var logDirectory = string.IsNullOrEmpty(logConfiguration.LogDirectory)
			    ? "../logs"
			    : logConfiguration.LogDirectory;

		    var logDirectoryLocal = string.IsNullOrEmpty(logConfiguration.LogDirectoryLocal)
			    ? logDirectory
			    : logConfiguration.LogDirectoryLocal;

		    var envAppName = GetAppName(env);

			return new NLogConfiguration
		    {
			    EnableWebLogs = logConfiguration.EnableWebLogs,
			    EnableAllLogs = logConfiguration.EnableAllLogs,
			    LogDirectory = logDirectory,
			    LogDirectoryLocal = logDirectoryLocal,
				AppName = string.IsNullOrEmpty(logConfiguration.AppName) ? envAppName : logConfiguration.AppName
		    };
	    }

	    private static NLogConfiguration DefaultNLogConfiguration(IHostingEnvironment env)
	    {
		    var envnName = GetEnvironmentName(env);
		    var appName = GetAppName(env);

		    return new NLogConfiguration
		    {
			    EnableWebLogs = true,
			    EnableAllLogs = envnName.Equals("Local") || envnName.Equals("Development"),
			    LogDirectory = "../logs",
			    LogDirectoryLocal = "logs",
			    AppName = appName
		    };
	    }

	    private static string GetAppName(IHostingEnvironment env)
	    {
		    return !string.IsNullOrEmpty(env?.ApplicationName) ? env.ApplicationName : "CustomApp";
	    }

	    private static string GetEnvironmentName(IHostingEnvironment env)
	    {
		    return !string.IsNullOrEmpty(env?.EnvironmentName) ? env.EnvironmentName : "Local";
	    }

		//==========================================================================================
	    //HACK: track whether DEBUG is enabled (could not find a built-in way to do this)
	    private static List<string> _listDebugEnabled = new List<string>();

	    public static void TrackDebugEnabled(string key, LogLevel logLevel)
	    {
		    if (logLevel <= LogLevel.Debug && !_listDebugEnabled.Contains(key))
		    {
			    _listDebugEnabled.Add(key);
		    }
	    }

	    public static bool IsDebugEnabledFor(string key)
	    {
		    return _listDebugEnabled.Contains(key);
	    }


	}
}
