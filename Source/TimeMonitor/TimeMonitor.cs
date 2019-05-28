using System;
using System.Diagnostics;
using PerfectCode.Logging;

namespace PerfectCode.TimeMonitoring
{
    public class TimeMonitor : IDisposable
    {
        private readonly string _message;
        private readonly int _allowedDurationInMilliseconds;
        private static readonly ILogger Log = new Logger(typeof(TimeMonitor));
        private readonly Stopwatch _stopwatch;

        public TimeMonitor(string message, int allowedDurationInMilliseconds = 0)
        {
            _message = message;
            _allowedDurationInMilliseconds = allowedDurationInMilliseconds;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            var elapsed = _stopwatch.ElapsedMilliseconds;

            if (elapsed > _allowedDurationInMilliseconds || _allowedDurationInMilliseconds == 0)
            {
                Log.Debug($"{_message} executed in {elapsed} ms");
            }
            _stopwatch.Stop();
        }
    }
}
