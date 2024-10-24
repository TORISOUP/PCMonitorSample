using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PcMonitor
{
    public static class Program
    {
        public static async Task Main()
        {
            try
            {
                // HardwareObserverの作成
                using var hardwareObserver = new HardwareObserver();

                // Configファイルの読み込み
                var conf = GetConfiguration() ?? throw new Exception("Configuration is null");

                // Configファイルを用いて初期化
                var dbSender = new InfluxDbSender(conf.InfluxDbUrl, conf.Token, conf.Bucket, conf.Organization);

                // 転送開始
                await SendMachineInfoAsync(dbSender, hardwareObserver, conf);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 定期的にInfluxDBにマシン情報を送信する
        /// </summary>
        private static async Task SendMachineInfoAsync(InfluxDbSender influxDbSender,
            HardwareObserver hardwareObserver,
            Configuration conf)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    // マシン情報を取得して送信
                    var machineInfo = hardwareObserver.GetMachineInfo();
                    await influxDbSender.SendMachineInfoAsync(machineInfo, default);

                    retryCount = 0;
                    await Task.Delay(conf.IntervalMilliSeconds ?? 5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    
                    // InfluxDBへの送信に失敗した場合はしばらく待ってリトライ
                    // リトライ間隔は徐々に伸ばす
                    retryCount++;
                    if (retryCount > 20) return;
                    await Task.Delay(TimeSpan.FromMinutes(retryCount));
                }
            }
        }
        
        /// <summary>
        /// exeファイルがあるディレクトリにあるconfig.jsonを読み込む
        /// </summary>
        private static Configuration? GetConfiguration()
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appDir = Path.GetDirectoryName(appPath);
            var path = Path.Combine(appDir, "config.json");
            if (!File.Exists(path))
            {
                // 存在しないならデフォルト値で作る
                var conf = new Configuration();
                var j = JsonConvert.SerializeObject(conf);
                File.WriteAllText(path, j);
                return conf;
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Configuration>(json);
        }
    }
}