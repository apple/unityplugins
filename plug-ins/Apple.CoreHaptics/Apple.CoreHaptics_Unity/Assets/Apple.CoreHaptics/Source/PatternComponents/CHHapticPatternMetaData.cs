using System;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticPatternMetaData : ISerializable
    {
        public string Project;
        public string Created;
        public string Description;

        public bool HasMetadata()
        {
            return !string.IsNullOrEmpty(Project) || !string.IsNullOrEmpty(Created) || !string.IsNullOrEmpty(Description);
        }

        public string Serialize(Serializer serializer)
        {
            var ret = "{";
            var needsPrecedingComma = false;
            if (!string.IsNullOrEmpty(Project))
            {
                ret += needsPrecedingComma ? ",\n" : "\n";
                ret += $"\t\t\"Project\": \"{Project}\"";
                needsPrecedingComma = true;
            }
            if (!string.IsNullOrEmpty(Created))
            {
                ret += needsPrecedingComma ? ",\n" : "\n";
                ret += $"\t\t\"Created\": \"{Created}\"";
                needsPrecedingComma = true;
            }
            if (!string.IsNullOrEmpty(Description))
            {
                ret += needsPrecedingComma ? ",\n" : "\n";
                ret += $"\t\t\"Description\": \"{Description}\"";
            }
            ret += "\t}";
            return ret;
        }
    }
}
