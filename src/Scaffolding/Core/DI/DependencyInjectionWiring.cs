using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lamar;
using Scaffolding.Core.DI.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Scaffolding.Core.DI
{
    public static class DependencyInjectionWiring
    {
	   public static IConfigurationRoot BuildConfig(string environment = null, string basePath = null)
	    {
		    IConfigurationBuilder builder = new ConfigurationBuilder();
		    string environmentVal = environment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
		    string globalConfigFilePath = Environment.GetEnvironmentVariable("AEGIS_CONFIG_PATH");

		    if (string.IsNullOrEmpty(globalConfigFilePath))
		    {
			    Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Environment Variable to access global config file is not set. Please run the ConfigurationDeployer.");
				Console.ResetColor();
		    }

		    string globalSettingsFile = $"{globalConfigFilePath}globalSettings.{environmentVal}.json";

			Console.WriteLine($"Initializing application in Environment {environmentVal}.  From runtime: {environment}; EnvironmentVariable: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")};");
			Console.WriteLine($"Global Config File Path: {globalConfigFilePath}");

			builder.SetBasePath(!string.IsNullOrEmpty(basePath) ? basePath : Directory.GetCurrentDirectory())
				.AddJsonFile(globalSettingsFile, optional: true, reloadOnChange: true)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			    .AddJsonFile($"appsettings.{environmentVal}.json", optional: true)
				.AddEnvironmentVariables();

			Console.WriteLine($"Provided base path: {basePath}; Current Directory: {Directory.GetCurrentDirectory()}");

		    return builder.Build();
		}

	    public static ServiceRegistry InitializeIOC(this ServiceRegistry services)
	    {
		    // Register stuff in container, using the StructureMap APIs...
		    services.Scan(_ =>
		    {
			    _.TheCallingAssembly();
				_.AssembliesFromApplicationBaseDirectory(a =>
				{
					return a.GetName().Name.IndexOf("CRM.") == 0 ||
					       a.GetName().Name.IndexOf("IdentityHub.") == 0 ||
					       a.GetName().Name.IndexOf("Messaging.") == 0 ||
					       a.GetName().Name.IndexOf("Scaffolding.") == 0 ||
					       a.GetName().Name.IndexOf("WebApiHub.") == 0;
				});
			    _.WithDefaultConventions();
		    });
			
		    return services;
	    }
    }
}
