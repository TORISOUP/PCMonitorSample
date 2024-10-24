using System;

namespace PcMonitor
{
    [Serializable]
    public class Configuration
    {
        public string? InfluxDbUrl = "http://localhost:8086";
        public string? Token = "my-token";
        public string? Organization = "my-org";
        public string? Bucket = "my-bucket";
        public int? IntervalMilliSeconds = 5000;
    }
}