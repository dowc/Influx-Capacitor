﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InfluxDB.Net;
using InfluxDB.Net.Models;
using Tharga.InfluxCapacitor.Collector.Interface;

namespace Tharga.InfluxCapacitor.Collector.Business
{
    public class MetaDataBusiness : IMetaDataBusiness
    {
        public static async Task<InfluxDbApiResponse> TestWriteAccess(IInfluxDbAgent client, string action)
        {
            var tags = new Dictionary<string, string>
            {
                { "counter", "configuration" },
                { "hostname", Environment.MachineName },
                { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { "action", action },
            };

            var fields = new Dictionary<string, object>
            {
                { "value", (decimal)1.0 },
            };

            var points = new[]
            {
                new Point
                    {
                        Name = Constants.ServiceName + "-Metadata", 
                        Tags = tags,
                        Fields = fields,
                        Precision = TimeUnit.Milliseconds,
                        Timestamp = DateTime.UtcNow
                    },
            };

            return await client.WriteAsync(points);
        }

        public static Point GetQueueCountPoints(string action, string targetServer, string targetDatabase, int previousQueueCount, int queueCountChange, Tuple<string,double?> response)
        {
            var tags = new Dictionary<string, string>
            {
                { "counter", "queueCount" },
                { "hostname", Environment.MachineName },
                { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { "action", action },
                { "targetServer", targetServer },
                { "targetDatabase", targetDatabase },
            };

            if (!string.IsNullOrEmpty(response.Item1))
            {
                tags.Add("failMessage", response.Item1);
            }

            var fields = new Dictionary<string, object>
            {
                { "value", (decimal)(previousQueueCount + queueCountChange) },
                //{ "queueCount", previousQueueCount + queueCountChange },
                { "queueCountChange", queueCountChange },
            };

            if (response.Item2 != null)
            {
                fields.Add("sendTimeMs", (decimal)response.Item2.Value);
            }

            var point = new Point
            {
                Name = Constants.ServiceName + "-Metadata",
                Tags = tags,
                Fields = fields,
                Precision = TimeUnit.Milliseconds,
                Timestamp = DateTime.UtcNow
            };
            
            return point;
        }

        public static IEnumerable<Point> GetCollectorPoint(string engineName, string performanceCounterGroup, int counters, Dictionary<string, long> timeInfo, double? elapseOffsetMs)
        {
            var tags = new Dictionary<string, string>
            {
                { "counter", "readCount" },
                { "hostname", Environment.MachineName },
                { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { "action", "collect" },
                { "performanceCounterGroup", performanceCounterGroup },
                { "engineName", engineName },
            };

            var fields = new Dictionary<string, object>
            {
                { "value", (decimal)counters },
                //{ "readCount", counters }
            };

            if (elapseOffsetMs != null)
            {
                fields.Add("elapseOffsetTimeMs", (decimal)elapseOffsetMs.Value);
            }

            var totalTimeMs = timeInfo.Sum(x => x.Value);
            fields.Add("totalTimeMs", (decimal)new TimeSpan(totalTimeMs).TotalMilliseconds);

            var now = DateTime.UtcNow;

            yield return new Point
            {
                Name = Constants.ServiceName + "-Metadata",
                Tags = tags,
                Fields = fields,
                Precision = TimeUnit.Milliseconds,
                Timestamp = now
            };

            var index = 0;
            foreach (var ti in timeInfo)
            {
                index++;

                fields = new Dictionary<string, object>
                {
                    { "value", (decimal)new TimeSpan(ti.Value).TotalMilliseconds },
                    //{ "readTime", (decimal)new TimeSpan(ti.Value).TotalMilliseconds }
                };

                tags = new Dictionary<string, string>
                {
                    { "counter", "readTime" },
                    { "hostname", Environment.MachineName },
                    { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                    { "action", "collect" },
                    { "performanceCounterGroup", performanceCounterGroup },
                    { "engineName", engineName },
                    { "step", index + "-" + ti.Key },
                };

                yield return new Point
                {
                    Name = Constants.ServiceName + "-Metadata",
                    Tags = tags,
                    Fields = fields,
                    Precision = TimeUnit.Milliseconds,
                    Timestamp = now
                };
            }
        }
    }
}