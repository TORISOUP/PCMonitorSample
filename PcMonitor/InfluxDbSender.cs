using System;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using PcMonitor.Protocol;

namespace PcMonitor
{
    public sealed class InfluxDbSender : IDisposable
    {
        private readonly InfluxDBClient _influxDbClient;
        private readonly string _bucket;
        private readonly string _organization;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="url">例: http://localhost:8086 </param>
        /// <param name="token"> 3-2で作ったAPI Token </param>
        /// <param name="bucket"> 3-1で作ったBucketの名前 </param>
        /// <param name="organization"> DOCKER_INFLUXDB_INIT_ORG で指定したorganization名 </param>
        public InfluxDbSender(string url, string token, string bucket, string organization)
        {
            // クライント初期化
            _influxDbClient = new InfluxDBClient(url, token);
            _bucket = bucket;
            _organization = organization;
        }

        public async ValueTask SendMachineInfoAsync(MachineInfo machineInfo, CancellationToken ct)
        {
            try
            {
                // 非同期用のWriter取得
                var writeApi = _influxDbClient.GetWriteApiAsync();

                // CPU状態の書き込み
                if (machineInfo.Cpu != null)
                {
                    var cpu = new CPU
                    {
                        Time = DateTime.UtcNow,
                        Name = machineInfo.Cpu.Name,
                        Temperature = machineInfo.Cpu.Temperature.Current,
                        Load = machineInfo.Cpu.Load.Current,
                        Watt = machineInfo.Cpu.Watt.Current
                    };

                    await writeApi.WriteMeasurementAsync(cpu, WritePrecision.S, bucket: _bucket, org: _organization,
                        cancellationToken: ct);
                }

                // メモリ状態の書き込み
                if (machineInfo.Memory != null)
                {
                    var memory = new Memory
                    {
                        Time = DateTime.UtcNow,
                        Name = machineInfo.Memory.Name,
                        Load = machineInfo.Memory.Load.Current!
                    };
                    await writeApi.WriteMeasurementAsync(memory, WritePrecision.S, bucket: _bucket, org: _organization,
                        cancellationToken: ct);
                }

                // GPU状態の書き込み
                if (machineInfo.Gpu != null)
                {
                    var gpu = new GPU
                    {
                        Time = DateTime.UtcNow,
                        Name = machineInfo.Gpu.Name,
                        Temperature = machineInfo.Gpu.Temperature.Current!,
                        Load = machineInfo.Gpu.Load.Current!,
                        Watt = machineInfo.Gpu.Watt.Current!
                    };

                    await writeApi.WriteMeasurementAsync(gpu, WritePrecision.S, bucket: _bucket, org: _organization,
                        cancellationToken: ct);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            _influxDbClient.Dispose();
        }
    }

    [Measurement("CPU")]
    public class CPU
    {
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }

        [Column("Name", IsTag = true)] public string Name { get; set; }
        [Column("Temperature")] public double? Temperature { get; set; }
        [Column("Load")] public double? Load { get; set; }
        [Column("Watt")] public double? Watt { get; set; }
    }

    [Measurement("Memory")]
    public class Memory
    {
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }
        [Column("Name", IsTag = true)] public string Name { get; set; }
        [Column("Load")] public double? Load { get; set; }
    }

    [Measurement("GPU")]
    public class GPU
    {
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }
        [Column("Name", IsTag = true)] public string Name { get; set; }
        [Column("Temperature")] public double? Temperature { get; set; }
        [Column("Load")] public double? Load { get; set; }
        [Column("Watt")] public double? Watt { get; set; }
    }

    [Measurement("Network")]
    public class Network
    {
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }
        [Column("Name", IsTag = true)] public string Name { get; set; }
        [Column("SentByte")] public long Sent { get; set; }
        [Column("ReceivedByte")] public long Received { get; set; }
    }
}