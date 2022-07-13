using System;
using System.IO;
using System.Linq;

using UnityEngine;
using Apple.UnityJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apple.CoreHaptics
{
	public static class CHSerializer
	{
		public static string Serialize(CHHapticPattern ahap)
		{
			return JSON.Serialize(ahap);
		}

		public static CHHapticPattern Deserialize(string ahapJson)
		{
			var ahap = JSON.Deserialize<CHHapticPattern>(ahapJson);
			foreach (var patternEntry in ahap.Pattern)
			{
				if (patternEntry.IsEvent() && patternEntry.Event.EventType.ToString() == "AudioCustom" && !(patternEntry.Event.EventWaveformPath is null))
				{

					var waveFileImportPath = patternEntry.Event.EventWaveformPath;
					if (waveFileImportPath.Contains("StreamingAssets/"))
					{
						var parts = waveFileImportPath.Split(new[] { "StreamingAssets/" }, StringSplitOptions.RemoveEmptyEntries);
						waveFileImportPath = parts[parts.Length - 1];
					}
#if UNITY_EDITOR
					waveFileImportPath = Path.Combine("Assets/StreamingAssets", waveFileImportPath);
#else
                    waveFileImportPath = Path.Combine(Application.streamingAssetsPath, waveFileImportPath);
#endif

					if (!File.Exists(waveFileImportPath))
					{
						Debug.LogError($"AHAP Import Error: Could not find {patternEntry.Event.EventWaveformPath}. Please import it to {waveFileImportPath}.");
						continue;
					}

					if (!CHHapticAudioCustomEvent.SupportedAudioExtensions.Any(waveFileImportPath.EndsWith))
					{
						Debug.LogWarning($"Audio file {waveFileImportPath} has an " +
							"unsupported extension. It may not play properly.");
					}
					patternEntry.Event.EventWaveformPath = waveFileImportPath;
#if UNITY_EDITOR
					patternEntry.Event.EventWaveform = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(waveFileImportPath);
					if (patternEntry.Event.EventWaveform is null)
					{
						Debug.LogWarning("Unable to populate the EventWaveform field for this Event.\n" +
							"However, your asset path is valid and this should not result in any playback issues.");
					}
#endif
				}
			}
			return ahap;
		}

		/// <summary>
		/// Serializes the prepared pattern for use
		/// in runtime scenarios where waveform path is
		/// replaced to ensure correct streaming assets path.
		/// </summary>
		/// <param name="ahap"></param>
		/// <returns></returns>
		public static string SerializeForRuntime(CHHapticPattern ahap)
		{
			// Prepare events...
			foreach (var entry in ahap.Pattern)
			{
				if (entry.IsEvent())
				{
					var e = entry.Event;
					if (e.EventType == CHHapticEventType.AudioCustom)
					{
						VerifyStreamingAssetsPathForEventWaveformPath(e);
					}
				}
			}

			return ahap.ToJSONString();
		}

		private static void VerifyStreamingAssetsPathForEventWaveformPath(CHHapticEvent hapticEvent)
		{
			if (string.IsNullOrEmpty(hapticEvent.EventWaveformPath))
				throw new CHSerializerException("CHSerializer: No EventWaveformPath specified for CHHapticEvent!");

			if (!hapticEvent.EventWaveformPath.StartsWith(Application.streamingAssetsPath))
			{
				hapticEvent.EventWaveformPath = Path.Combine(Application.streamingAssetsPath, hapticEvent.EventWaveformPath);
			}

			if (!File.Exists(hapticEvent.EventWaveformPath))
			{
				throw new CHSerializerException($"CHSerializer: No file found at path '{hapticEvent.EventWaveformPath}' for CHHapticEvent!");
			}
		}
	}
}
