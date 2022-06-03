using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// A class representing a sound event blend node in PHASE.
    /// </summary>
    [NodeWidth(300)]
    public class PHASESoundEventBlendNode : PHASESoundEventNode
    {
        // Limit a max of blend entries, 10 should be more than enough.
        private static int _kMaxBlendEntries = 10;

        /// <summary>
        /// A struct representing a blend node entry in the Sound Event Composer.
        /// </summary>
        [System.Serializable]
        public struct Entry
        {
            public string PortName;

            // Fade parameters
            public float LowValue;
            public float FullGainAtLow;
            public float FullGainAtHigh;
            public float HighValue;
        }

        /// <summary>
        /// A <c>List</c> of all blend node entries on this node.
        /// </summary>
        [SerializeField]
        public List<Entry> Entries = new List<Entry>();

        // Use Distance Blend Mode or Parameter
        /// <summary>
        /// Set true to use a spatial mixer to control blending.
        /// </summary>
        public bool UseDistanceBlend = false;

        // Parameter to blend on
        /// <summary>
        /// A parameter of type double to control blending.
        /// </summary>
        /// <remarks> Only used with <c>UseDistanceBlend</c> is false. </remarks>
        [SerializeField] public PHASESoundEventParameter BlendParameter = null;
        [Output] public PHASESoundEventParameterDouble Parameter = null;

        /// <summary>
        /// The spatial mixer to control blending.
        /// </summary>
        /// <remarks> Only used with <c>UseDistanceBlend</c> is true. </remarks>
        [SerializeField] private PHASEMixer _distanceBlendSpatialMixer = null;
        [Output] public PHASEMixer Mixer = null;

        /// <summary>
        /// Create the blend node in the PHASE engine.
        /// </summary>
        /// <returns> True on success, false otherwise. </returns>
        public override bool Create()
        {
            _distanceBlendSpatialMixer = GetMixerFromPort();
            BlendParameter = GetParameterFromPort();

            // Create blend parameter.
            if (!UseDistanceBlend && (!BlendParameter || !BlendParameter.Create()))
            {
                Debug.LogError("Cannot create Blend Node without Parameter or Mixer.");
                return false;
            }

            if (UseDistanceBlend && _distanceBlendSpatialMixer == null)
            {
                Debug.LogError("No Distance Blend Spatial Mixer on " + name + ".");
                return false;
            }

            // Create all the nodes this blend node contains
            Helpers.BlendNodeEntry[] entries = new Helpers.BlendNodeEntry[_kMaxBlendEntries];
            for (int entryIdx = 0; entryIdx < Entries.Count; ++entryIdx)
            {
                PHASESoundEventNode node = GetPort(Entries[entryIdx].PortName).GetConnection(0).node as PHASESoundEventNode;
                bool result = node.Create();
                if (result == false)
                {
                    Debug.LogError("Failed to create subnodes of blend action tree node.");
                }

                entries[entryIdx].LowValue = Entries[entryIdx].LowValue;
                entries[entryIdx].FullGainAtLow = Entries[entryIdx].FullGainAtLow;
                entries[entryIdx].FullGainAtHigh = Entries[entryIdx].FullGainAtHigh;
                entries[entryIdx].HighValue = Entries[entryIdx].HighValue;
                entries[entryIdx].NodeId = node.GetNodeId();
            }

            // Now create the blend node with all the entries 
            long paramId = UseDistanceBlend ? _distanceBlendSpatialMixer.GetMixerId() : BlendParameter.GetParameterId();
            m_nodeId = Helpers.PHASECreateSoundEventBlendNode(paramId, entries, Entries.Count, UseDistanceBlend);
            if (m_nodeId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create PHASE blend node.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Destroy the blend node in the PHASE engine.
        /// </summary>
        public override void DestroyFromPHASE()
        {
            if (Entries.Count > 0)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    PHASESoundEventNode node = GetPort(Entries[i].PortName).GetConnection(0).node as PHASESoundEventNode;
                    node.DestroyFromPHASE();
                }
            }

            if (BlendParameter != null)
            {
                BlendParameter.DestroyFromPHASE();
            }

            if (_distanceBlendSpatialMixer != null)
            {
                _distanceBlendSpatialMixer.DestroyFromPHASE();
            }

            base.DestroyFromPHASE();
        }

        /// <summary>
        /// Returns a <c>List</c> of all mixers associated with this blend node.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this blend node.</returns>
        public override List<PHASEMixer> GetMixers()
        {
            List<PHASEMixer> mixers = new List<PHASEMixer>();
            _distanceBlendSpatialMixer = GetMixerFromPort();
            if (_distanceBlendSpatialMixer != null)
            {
                mixers.Add(_distanceBlendSpatialMixer);
            }
            for (int i = 0; i < Entries.Count; i++)
            {
                PHASESoundEventNode node = GetPort(Entries[i].PortName).GetConnection(0).node as PHASESoundEventNode;
                foreach (PHASEMixer entry in node.GetMixers())
                {
                    if (!mixers.Contains(entry))
                    {
                        mixers.Add(entry);
                    }
                }
            }
            return mixers;
        }

#if UNITY_EDITOR
        public void RemoveEntry(int index)
        {
            RemoveDynamicPort(Entries[index].PortName);
            Entries.RemoveAt(index);
        }

        public void AddEntry()
        {
            if (Entries.Count >= _kMaxBlendEntries)
            {
                Debug.LogWarning("Did not create new BlendEntry. Max number of entries met.");
                return;
            }
            NodePort port = AddDynamicOutput(typeof(PHASESoundEventNode), ConnectionType.Override, TypeConstraint.Strict);
            Entries.Add(new Entry() { PortName = port.fieldName, LowValue = 0, HighValue = 0, FullGainAtLow = 0, FullGainAtHigh = 0, });
        }
#endif
    }
}
