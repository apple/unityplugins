using System;
using System.Collections.Generic;
using UnityEngine;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
	[Serializable]
	public class CHHapticPattern : ISerializable
	{
		public int Version;
		public CHHapticPatternMetaData Metadata;
		public List<CHHapticPatternComponent> Pattern;

		public CHHapticPattern()
		{
			Version = 1;
			Metadata = new CHHapticPatternMetaData();
			Pattern = new List<CHHapticPatternComponent>();
		}

		public CHHapticPattern(List<CHHapticEvent> events) : this()
		{
			foreach (var e in events)
			{
				Pattern.Add(new CHHapticPatternComponent(e));
			}
		}

		public CHHapticPattern(List<CHHapticEvent> events, List<CHHapticParameter> patternParams) : this(events)
		{
			foreach (var p in patternParams)
			{
				Pattern.Add(new CHHapticPatternComponent(p));
			}
		}

		public CHHapticPattern(List<CHHapticEvent> events, List<CHHapticParameterCurve> paramCurves) : this(events)
		{
			foreach (var pc in paramCurves)
			{
				Pattern.Add(new CHHapticPatternComponent(pc));
			}
		}

		public CHHapticPattern(List<CHHapticEvent> events, List<CHHapticParameter> patternParams, List<CHHapticParameterCurve> paramCurves) : this(events)
		{
			foreach (var p in patternParams)
			{
				Pattern.Add(new CHHapticPatternComponent(p));
			}

			foreach (var pc in paramCurves)
			{
				Pattern.Add(new CHHapticPatternComponent(pc));
			}
		}

		public CHHapticPattern(TextAsset ahapTextAsset)
		{
			var deserializedAhap = CHSerializer.Deserialize(ahapTextAsset.text);
			Version = deserializedAhap.Version;
			Metadata = deserializedAhap.Metadata;
			Pattern = deserializedAhap.Pattern;
		}

		public CHHapticPattern(string ahapText)
		{
			var deserializedAhap = CHSerializer.Deserialize(ahapText);
			Version = deserializedAhap.Version;
			Metadata = deserializedAhap.Metadata;
			Pattern = deserializedAhap.Pattern;
		}

		public string Serialize(Serializer serializer)
		{
			var ret = "{\n";
			ret += $"\t\"Version\": {Version},\n";
			if (Metadata.HasMetadata())
			{
				ret += $"\t\"Metadata\": {Metadata.ToJSONString()},\n";
			}
			ret += "\t\"Pattern\": [";
			var needsPrecedingComma = false;
			foreach (var comp in Pattern)
			{
				ret += needsPrecedingComma ? ",\n" : "\n";
				ret += "\t\t{\n";

				if (!(comp.Event is null))
				{
					ret += "\t\t\t\"Event\": ";
					switch (comp.Event.EventType)
					{
						case CHHapticEventType.AudioContinuous:
							ret += new CHHapticAudioContinuousEvent(comp.Event).ToJSONString();
							break;
						case CHHapticEventType.AudioCustom:
							ret += new CHHapticAudioCustomEvent(comp.Event).ToJSONString();
							break;
						case CHHapticEventType.HapticContinuous:
							ret += new CHHapticContinuousEvent(comp.Event).ToJSONString();
							break;
						case CHHapticEventType.HapticTransient:
							ret += new CHHapticTransientEvent(comp.Event).ToJSONString();
							break;
					}
				}
				else if (!(comp.Parameter is null))
				{
					ret += "\t\t\t\"Parameter\": ";
					ret += comp.Parameter.ToJSONString();
				}
				else if (!(comp.ParameterCurve is null))
				{
					ret += "\t\t\t\"ParameterCurve\": ";
					ret += comp.ParameterCurve.ToJSONString();
				}
				else
				{
					Debug.LogWarning("Unknown AHAP Pattern element.");
				}

				ret += "\t\t}";
				needsPrecedingComma = true;
			}
			ret += "\n\t]\n}";
			return ret;
		}
	}
}
