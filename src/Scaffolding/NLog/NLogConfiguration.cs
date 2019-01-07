using System.Collections.Generic;
using NLog;
using NLog.Config;

namespace Scaffolding.NLog
{
    public class NLogConfiguration : ILogConfiguration
    {
	    public bool EnableAllLogs { get; set; } = false;
	    public bool EnableWebLogs { get; set; } = true;
		public string LogDirectory { get; set; }
		public string LogDirectoryLocal { get; set; }
		public string AppName { get; set; }
	    public LogLevel LogLevel { get; set; } = null;
		public List<LoggingRule> LoggingRules { get; set; } = null;
	}
}
