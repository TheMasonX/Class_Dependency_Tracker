using Serilog;
using Serilog.Events;

namespace ClassDependencyTracker.Tests;

[SetUpFixture]
public class SetupTrace
{

    private string _logFile = $"./ClassDependencyTracker.Log";
    private LogEventLevel _traceLogLevel = LogEventLevel.Verbose;

    [OneTimeSetUp]
    public void StartTest()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.File(_logFile)
            .WriteTo.Trace(_traceLogLevel)
            .CreateLogger();
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }
}
