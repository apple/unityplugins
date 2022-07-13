using System;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticContinuousEvent : CHHapticEvent {
        public void SetEventDuration(float val) {
            EventDuration = Math.Max(val, 0);
        }

        public CHHapticContinuousEvent() {
            EventType = CHHapticEventType.HapticContinuous;
            Time = 0f;
            EventDuration = 1f;
        }

        public CHHapticContinuousEvent(CHHapticEvent e) {
            EventType = CHHapticEventType.HapticContinuous;
            Time = e.Time;
            EventDuration = e.EventDuration;
            EventParameters = e.EventParameters;
        }

        public override string Serialize(Serializer serializer) {
            var ret = "{\n";
            ret += SerializeTypeAndTime();
            ret += $",\n\t\t\t\t\"EventDuration\": {EventDuration}";

            ret += SerializeEventParams();

            ret += "\t\t\t}\n";
            return ret;
        }
    }
}
