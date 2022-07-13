using System;
using System.IO;

using Apple.UnityJSON;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticAudioCustomEvent : CHHapticEvent {
        public static string[] SupportedAudioExtensions = {
            ".aac", ".adts", ".ac3", ".aif", ".aifc", ".aiff", ".caf",
            ".mp3", ".mp4", ".m4a", ".snd", ".au", ".sd2", ".wav"
        };

        public void SetEventDuration(float val) {
            EventDuration = Math.Max(val, 0);
        }

        public void SetEventWaveform(UnityEngine.Object waveformAsset) {
            EventWaveform = waveformAsset;
        }

        public void SetEventWaveformPath(string path) {
            var tmp = path;

            if (string.IsNullOrEmpty(tmp)) {
#if UNITY_EDITOR // Allow empty waveforms in the editor only
                EventWaveformPath = tmp;
                EventWaveform = null;
                return;
#else
                throw new ArgumentException("Cannot set the EventWaveformPath to empty.");
#endif
            }

            if (tmp.Contains("StreamingAssets/")) {
                var parts = tmp.Split(new[] { "StreamingAssets/" }, StringSplitOptions.RemoveEmptyEntries);
                tmp = parts[parts.Length - 1];
            }

#if !UNITY_EDITOR
			if (!tmp.StartsWith(Application.streamingAssetsPath)) {
                tmp = Application.streamingAssetsPath + "/" + tmp;
            }

            if (!File.Exists(tmp))
#else
            if (!File.Exists(Path.Combine("Assets/StreamingAssets", tmp)))
#endif
            {
                throw new ArgumentException($"The audio file {tmp} does not exist. " +
                    "Please be sure to place your audio file in the StreamingAssets directory.");
            }
#if UNITY_EDITOR
            EventWaveform = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine("Assets/StreamingAssets", tmp));
            if (EventWaveform is null) {
                Debug.LogWarning($"Unable to load audio file at {EventWaveformPath} for some reason.");
            }
#endif
            EventWaveformPath = tmp;
        }

        public CHHapticAudioCustomEvent() {
            EventType = CHHapticEventType.AudioCustom;
            Time = 0f;
            EventWaveformPath = "";
        }

        public CHHapticAudioCustomEvent(CHHapticEvent e) {
            EventType = CHHapticEventType.AudioCustom;
            Time = e.Time;
            SetEventDuration(e.EventDuration);
            SetEventWaveformPath(e.EventWaveformPath);
            EventParameters = e.EventParameters;
        }

        public override string Serialize(Serializer serializer) {
            var ret = "{\n";
            ret += SerializeTypeAndTime();
            ret += ",\n";
            if (EventDuration > 0) {
                ret += $"\t\t\t\t\"EventDuration\": {EventDuration},\n";
            }

            ret += $"\t\t\t\t\"EventWaveformPath\": \"{EventWaveformPath}\"";

            ret += SerializeEventParams();

            ret += "\t\t\t}\n";
            return ret;
        }
    }
}
