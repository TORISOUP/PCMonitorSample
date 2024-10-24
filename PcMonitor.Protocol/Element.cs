using System;

namespace PcMonitor.Protocol
{
    [Serializable]
    public class Element
    {
        public string Type;
        public string Target;
        public float? Max;
        public float? Current;

        public Element(string type, string target, float? max, float? current)
        {
            Type = type;
            Target = target;
            Max = max;
            Current = current;
        }

        public override string ToString()
        {
            return
                $"{nameof(Type)}: {Type}, {nameof(Target)}: {Target}, {nameof(Max)}: {Max}, {nameof(Current)}: {Current}";
        }
    }
}