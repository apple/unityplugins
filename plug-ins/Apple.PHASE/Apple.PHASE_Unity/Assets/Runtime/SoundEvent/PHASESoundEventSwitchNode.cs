using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using XNode;
using System;

namespace Apple.PHASE
{
    /// <summary>
    /// Class representing a switch node in the PHASE engine.
    /// </summary>
    [NodeWidth(300)]
    public class PHASESoundEventSwitchNode : PHASESoundEventNode
    {
        /// <summary>
        /// Struct representing a switch node entry that is use with the Sound Event Composer.
        /// </summary>
        /// <remarks><c>SwitchValue</c> is a string that allows this child node to be selected by the associated switch parameter. </remarks>
        /// <see cref="_switchParameter"/>
        [Serializable]
        public struct Entry
        {
            public string PortName;
            public string SwitchValue;
        }

        /// <summary>
        /// A <c>List</c> containing all switch node entries 
        /// </summary>
        [SerializeField]
        public List<Entry> Entries = new List<Entry>();

        /// <summary>
        /// Set this parameters value to a string representing which switch node entry to select.
        /// </summary>
        [SerializeField] private PHASESoundEventParameter _switchParameter = null;
        [Output] public PHASESoundEventParameterString Parameter = null;

        /// <summary>
        /// Create this switch node in the PHASE engine.
        /// </summary>
        /// <returns> Returns true on success, false otherwise. </returns>
        public override bool Create()
        {
            _switchParameter = GetParameterFromPort();

            // Create switch parameter.
            if (!_switchParameter || !_switchParameter.Create())
            {
                Debug.LogError($"Failed to create SoundEvent. No MetaParameter on SwitchNode {name}.");
                return false;
            }

            // Create an array of entries that we can pass to the plugin
            Helpers.SwitchNodeEntry[] entries = new Helpers.SwitchNodeEntry[Entries.Count];

            // Create all the nodes this switch node contains
            for (int entryIdx = 0; entryIdx < Entries.Count; ++entryIdx)
            {
                PHASESoundEventNode node = GetPort(Entries[entryIdx].PortName).GetConnection(0).node as PHASESoundEventNode;
                bool result = node.Create();
                if (result == false)
                {
                    Debug.LogError("Failed to create subnodes of switch action tree node.");
                    return false;
                }

                // Store the node id and the weight
                entries[entryIdx].NodeId = node.GetNodeId();
                entries[entryIdx].SwitchValue = Entries[entryIdx].SwitchValue;
            }

            // Now create the switch node with all the entries 
            GCHandle gcEntries = GCHandle.Alloc(entries);
            m_nodeId = Helpers.PHASECreateSoundEventSwitchNode(_switchParameter.GetParameterId(), entries, (uint)entries.Length);
            if (m_nodeId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create PHASE switch node.");
                return false;
            }
            gcEntries.Free();

            return true;
        }

        /// <summary>
        /// Destroy this switch node from the PHASE engine.
        /// </summary>
        public override void DestroyFromPHASE()
        {
            // Destroy the nodes on all the entries in this switch node.
            if (Entries.Count > 0)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    PHASESoundEventNode node = GetPort(Entries[i].PortName).GetConnection(0).node as PHASESoundEventNode;
                    node.DestroyFromPHASE();
                }
            }

            base.DestroyFromPHASE();
        }

        /// <summary>
        /// Return a <c>List</c> of all mixers associated with this switch node.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this switch node.</returns>
        public override List<PHASEMixer> GetMixers()
        {
            List<PHASEMixer> mixers = new List<PHASEMixer>();
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
        public void AddEntry()
        {
            NodePort port = AddDynamicOutput(typeof(PHASESoundEventNode), ConnectionType.Override, TypeConstraint.Strict);
            Entries.Add(new Entry() { PortName = port.fieldName, SwitchValue = "Default" });
        }

        public void RemoveEntry(int index)
        {
            RemoveDynamicPort(Entries[index].PortName);
            Entries.RemoveAt(index);
        }
#endif
    }
}