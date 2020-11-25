using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Test_DotnetType
{
    public class PerformanceCounterListener: EventListener
    {
        private static HashSet<string> _keys = new HashSet<string>  { "Count", "Min", "Max", "Mean", "Increment" };
        private static DateTimeOffset? _lastSampleTime;

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            base.OnEventSourceCreated(eventSource);
            if (eventSource.Name == "System.Runtime")
            {
                EnableEvents(eventSource, EventLevel.Critical, (EventKeywords)(-1),  new Dictionary<string, string> { ["EventCounterIntervalSec"] = "5" });
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_lastSampleTime != null && DateTimeOffset.UtcNow - _lastSampleTime.Value > TimeSpan.FromSeconds(1))
            {
                Console.WriteLine();
            }
            _lastSampleTime = DateTimeOffset.UtcNow;
            var metrics = (IDictionary<string, object>)eventData.Payload[0];
            var name = metrics ["Name"];
            var values = metrics
                .Where(it=>_keys.Contains(it.Key))
                .Select(it => $"{it.Key} = {it.Value}");
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:mm::ss");
            Console.WriteLine($"[{timestamp}]{name, -32}: {string.Join("; ", values.ToArray())}");
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello World!");
            _ = new PerformanceCounterListener();
            Console.Read();
        }
    }
}