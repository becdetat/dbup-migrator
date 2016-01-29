using DbUp.Engine.Output;
using Serilog;

namespace DataMigrations
{
    public class SerilogUpgradeLog : IUpgradeLog
    {
        public void WriteInformation(string format, params object[] args)
        {
            Log.Information(format, args);
        }

        public void WriteError(string format, params object[] args)
        {
            Log.Error(format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            Log.Warning(format, args);
        }
    }
}