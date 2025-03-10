using System;
using System.Collections.Generic;
using static System.FormattableString;
using System.Globalization;
using UnityEngine;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticEvent : ICHHapticPatternEntry, ISerializable
    {
        public CHHapticEventType EventType;

        public float Time;
        public float EventDuration;
        public string EventWaveformPath;
        public UnityEngine.Object EventWaveform;

        public List<CHHapticEventParameter> EventParameters = new List<CHHapticEventParameter>();

		public virtual string Serialize(Serializer serializer) {
			throw new NotImplementedException();
		}

        internal string SerializeTypeAndTime() {
            var ret = $"\t\t\t\t\"EventType\": \"{EventType}\",\n";
            ret += ((FormattableString)$"\t\t\t\t\"Time\": {Time}").ToString(CultureInfo.InvariantCulture);;
            return ret;
        }

        internal string SerializeEventParams() {
            var ret = "";
            if (EventParameters.Count > 0) {
                ret += ",\n\t\t\t\t\"EventParameters\": [";

                for (var idx = 0; idx < EventParameters.Count; idx++) {
                    var ep = EventParameters[idx];
                    // Ensure string interpolation uses decimals, not commas
                    ret += ((FormattableString)$"\n\t\t\t\t\t{{\"ParameterID\": \"{ep.ParameterID}\", \"ParameterValue\": {ep.ParameterValue}}}").ToString(CultureInfo.InvariantCulture);

                    // Add a comma for all but the last Param
                    if (idx != EventParameters.Count - 1) {
                        ret += ",";
                    }
                }

                ret += "\n\t\t\t\t]\n";
            }
            else {
                ret += "\n";
            }

            return ret;
        }
	}
}
