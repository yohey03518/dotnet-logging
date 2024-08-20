namespace LoggingSdk;

public static class Constants
{
    public const string RequestIdHeader = "request-id";
    public const string LogTemplate = "[{Level:u}] {Timestamp:O} [{RequestId}] [{SourceContext}] {NewLine}{Message}{NewLine}{Exception}";
}