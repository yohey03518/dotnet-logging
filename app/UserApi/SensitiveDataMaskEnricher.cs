namespace UserApi;

using Serilog.Core;
using Serilog.Events;

public class SensitiveDataMaskEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var sensitiveProperties = new List<string>() { "mobile", "email" };

        foreach (var (key, value) in logEvent.Properties)
        {
            switch (value)
            {
                case ScalarValue scalarValue:
                {
                    if (sensitiveProperties.Any(sensitiveKey => key.Contains(sensitiveKey, StringComparison.OrdinalIgnoreCase)))
                    {
                        logEvent.AddOrUpdateProperty(new LogEventProperty(key, new ScalarValue(GetMaskValue(scalarValue))));
                    }

                    break;
                }
                case DictionaryValue dictionary:
                case SequenceValue sequence:
                {
                    // todo
                    break;
                }
                case StructureValue structure:
                {
                    var logEventProperties = structure.Properties.Select(property =>
                    {
                        if (sensitiveProperties.Any(sensitiveKey => property.Name.Contains(sensitiveKey, StringComparison.OrdinalIgnoreCase)))
                        {
                            return new LogEventProperty(property.Name, new ScalarValue(GetMaskValue((ScalarValue)property.Value)));
                        }

                        return property;
                    }).ToList();
                    logEvent.AddOrUpdateProperty(new LogEventProperty(key, new StructureValue(logEventProperties)));

                    break;
                }
            }
        }
    }

    private static string GetMaskValue(ScalarValue scalarValue)
    {
        var s = scalarValue.Value?.ToString();
        if (string.IsNullOrWhiteSpace(s))
        {
            return string.Empty;
        }
        var midIndex = s.Length/2;
        return s.Substring(0, midIndex) + "***";
    }
}