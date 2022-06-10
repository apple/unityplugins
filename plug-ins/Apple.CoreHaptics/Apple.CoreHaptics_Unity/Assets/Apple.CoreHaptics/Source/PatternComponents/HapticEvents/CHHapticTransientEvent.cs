using System;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticTransientEvent : CHHapticEvent {
        public CHHapticTransientEvent() {
            EventType = CHHapticEventType.HapticTransient;
            Time = 0f;
        }

        public CHHapticTransientEvent(CHHapticEvent e) {
            EventType = CHHapticEventType.HapticTransient;
            Time = e.Time;
            EventParameters = e.EventParameters;
        }

        public override string Serialize(Serializer serializer) {
            var ret = "{\n";
            ret += SerializeTypeAndTime();

            ret += SerializeEventParams();

            ret += "\t\t\t}\n";
            return ret;
        }
    }
}

