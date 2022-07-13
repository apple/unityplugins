using System;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticAudioContinuousEvent : CHHapticEvent {
        public void SetEventDuration(float val) {
            EventDuration = Math.Max(val, 0);
        }

        public CHHapticAudioContinuousEvent() {
            EventType = CHHapticEventType.AudioContinuous;
            Time = 0f;
            EventDuration = 1f;
        }

        public CHHapticAudioContinuousEvent(CHHapticEvent e) {
            EventType = CHHapticEventType.AudioContinuous;
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
