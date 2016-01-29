using System;
using System.Configuration;
using System.Linq;
using NDesk.Options;
using Serilog;

namespace DataMigrations
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var seqServer = ConfigurationManager.AppSettings["SeqServer"];
            var seqApiKey = ConfigurationManager.AppSettings["SeqApiKey"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .WriteTo.ColoredConsole(outputTemplate: "{Message}{NewLine}{Exception}")
                .WriteTo.Seq(seqServer, apiKey: seqApiKey)
                .CreateLogger();

            try
            {
                var options = new MigrationOptions();
                var showHelp = false;
                var optionSet = new OptionSet
                {
                    {"reset-the-world", "Drop databases and load sample data", x => options.ResetTheWorld = x != null},
                    {"connection-string=", "Connection string to operate on", x => options.ConnectionString = x},
                    {"help", "Show this message and exit", x => showHelp = x != null}
                };
                var extras = optionSet.Parse(args);

                if (extras.Any())
                {
                    Log.Error("Unknown parameters ({Extras}), try --help for more information", extras);
                    return 1;
                }

                if (showHelp)
                {
                    optionSet.WriteOptionDescriptions(Console.Out);
                    return 1;
                }

                var migrator = new Migrator(ConfigurationManager.AppSettings["DbConnection"]);

                if (!migrator.Boom(options.ResetTheWorld))
                {
                    Log.Fatal("Unknown error occured during migration");
                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unexpected error occurred");
                return 1;
            }
        }
    }
}