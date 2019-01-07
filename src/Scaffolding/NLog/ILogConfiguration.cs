using System.Collections.Generic;
using NLog;
using NLog.Config;

namespace Scaffolding.NLog
{
	public interface ILogConfiguration
	{
		bool EnableAllLogs { get; set; }
		bool EnableWebLogs { get; set; }
		string LogDirectory { get; set; }
		string LogDirectoryLocal { get; set; }
		string AppName { get; set; }
		LogLevel LogLevel { get; set; }
		List<LoggingRule> LoggingRules { get; set; }
	}
}