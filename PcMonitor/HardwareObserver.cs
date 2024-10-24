using System;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using PcMonitor.Protocol;

namespace PcMonitor
{
    // マシン状態の取得機構
    public sealed class HardwareObserver : IDisposable
    {
        private readonly Computer _computer = new();

        private readonly IHardware? _cpu;
        private readonly IHardware? _memory;
        private readonly IHardware? _gpu;

        public HardwareObserver()
        {
            // OpenHardwareMonitorの初期化
            _computer.Open();
            
            // CPU, メモリ, GPUの情報を取得する
            _computer.CPUEnabled = true;
            _computer.RAMEnabled = true;
            _computer.GPUEnabled = true;

            // Computer.Hardwareに取得したデバイス情報が詰まっている
            // ここからCPU, メモリ, GPUのデバイス情報を抜き取っておく
            foreach (var hardware in _computer.Hardware)
            {
                switch (hardware.HardwareType)
                {
                    case HardwareType.CPU:
                        _cpu = hardware;
                        break;
                    case HardwareType.RAM:
                        _memory = hardware;
                        break;
                    case HardwareType.GpuNvidia:
                    case HardwareType.GpuAti:
                        _gpu = hardware;
                        break;
                }
            }
        }

        /// <summary>
        /// マシン状態を取得する
        /// </summary>
        public MachineInfo GetMachineInfo()
        {
            _cpu?.Update();
            _memory?.Update();
            _gpu?.Update();

            var cpuInfo = GetCpuInfo();
            var memoryInfo = GetMemoryInfo();
            var gpuInfo = GetGpuInfo();

            return new MachineInfo(cpuInfo, memoryInfo, gpuInfo);
        }

        /// <summary>
        /// GPU状態の取得
        /// </summary>
        private GpuInfo? GetGpuInfo()
        {
            if (_gpu == null) return null;

            // GPU温度
            var gpuTemperature = _gpu.Sensors.FirstOrDefault(sensor =>
                sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core");
            // 負荷
            var gpuLoad = _gpu.Sensors.FirstOrDefault(sensor => sensor.SensorType == SensorType.Load
                                                                && sensor.Name == "GPU Core");
            // 消費電力
            var gpuPower = _gpu.Sensors.FirstOrDefault(sensor => sensor.SensorType == SensorType.Power
                                                                 && sensor.Name == "GPU Power");

            var gpuInfo = new GpuInfo(
                _gpu.Name,
                new Element("Temperature", gpuTemperature?.Name
                    , gpuTemperature?.Max, gpuTemperature?.Value),
                new Element("Load", gpuLoad?.Name, gpuLoad?.Max, gpuLoad?.Value),
                new Element("Watt", gpuPower?.Name, gpuPower?.Max, gpuPower?.Value)
            );

            return gpuInfo;
        }

        /// <summary>
        /// メモリ情報の取得
        /// </summary>
        private MemoryInfo? GetMemoryInfo()
        {
            if (_memory == null) return null;

            // メモリ使用率
            var memoryLoad = _memory.Sensors.FirstOrDefault(sensor => sensor.SensorType == SensorType.Load);

            var memoryInfo = new MemoryInfo(
                _memory.Name,
                new Element("Memory Load", _memory.Name, memoryLoad?.Max, memoryLoad?.Value)
            );
            return memoryInfo;
        }

        /// <summary>
        /// CPU情報の取得
        /// </summary>
        private CpuInfo? GetCpuInfo()
        {
            if (_cpu == null) return null;

            // CPU温度
            var cpuTemperature = _cpu.Sensors.FirstOrDefault(sensor =>
                sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package");

            // CPU負荷
            var cpuLoad = _cpu.Sensors.FirstOrDefault(sensor => sensor.SensorType == SensorType.Load
                                                                && sensor.Name == "CPU Total");
            // CPU消費電力
            var cpuWatt = _cpu.Sensors.FirstOrDefault(sensor => sensor.SensorType == SensorType.Power
                                                                && sensor.Name == "CPU Package");

            var cpuInfo = new CpuInfo(
                _cpu.Name,
                new Element("Temperature", cpuTemperature?.Name
                    , cpuTemperature?.Max, cpuTemperature?.Value),
                new Element("Load", cpuLoad?.Name, cpuLoad?.Max, cpuLoad?.Value),
                new Element("Watt", cpuWatt?.Name, cpuWatt?.Max, cpuWatt?.Value)
            );
            return cpuInfo;
        }

        public void Dispose()
        {
            _computer.Close();
        }
    }
}