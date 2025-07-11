using System.Windows;

using Serilog;
using Serilog.Events;

namespace VideoGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private string _logFile = $"./VideoGenerator.Log";
    private LogEventLevel _traceLogLevel = LogEventLevel.Verbose;

    public App () : base()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.File(_logFile)
            .WriteTo.Trace(_traceLogLevel)
            .CreateLogger();
    }
}