using System;

namespace PcMonitor.Protocol
{
    [Serializable]
    public class MachineInfo
    {
        public CpuInfo Cpu;
        public MemoryInfo Memory;
        public GpuInfo Gpu;
        
        public MachineInfo(CpuInfo cpu, MemoryInfo memory, GpuInfo gpu)
        {
            Cpu = cpu;
            Memory = memory;
            Gpu = gpu;
        }
        
        public override string ToString()
        {
            return $"{nameof(Cpu)}: {Cpu}, {nameof(Memory)}: {Memory}, {nameof(Gpu)}: {Gpu}";
        }
    }

    [Serializable]
    public class CpuInfo
    {
        public string Name;
        public Element Temperature;
        public Element Load;
        public Element Watt;

        public CpuInfo(string name, Element temperature, Element load, Element watt)
        {
            Name = name;
            Temperature = temperature;
            Load = load;
            Watt = watt;
        }
        
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Temperature)}: {Temperature}, {nameof(Load)}: {Load}, {nameof(Watt)}: {Watt}";
        }
    }

    [Serializable]
    public class MemoryInfo
    {
        public string Name;
        public Element Load;

        public MemoryInfo(string name, Element load)
        {
            Name = name;
            Load = load;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Load)}: {Load}";
        }
    }

    [Serializable]
    public class GpuInfo
    {
        public string Name;
        public Element Temperature;
        public Element Load;
        public Element Watt;
        
        public GpuInfo(string name, Element temperature, Element load, Element watt)
        {
            Name = name;
            Temperature = temperature;
            Load = load;
            Watt = watt;
        }
        
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Temperature)}: {Temperature}, {nameof(Load)}: {Load}, {nameof(Watt)}: {Watt}";
        }
    }
}