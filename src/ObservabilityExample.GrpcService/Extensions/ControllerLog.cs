namespace ObservabilityExample.GrpcService.Extensions;

public static partial class ControllerLog
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Starting execute method '{MethodName}' of '{ServiceName}' service ...")]
    public static partial void LogMethodExecution(this ILogger logger, string methodName, string serviceName);
}
