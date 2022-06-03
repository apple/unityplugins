using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// A class representing a sound event random node in PHASE.
    /// </summary>
    public class PHASESoundEventRandomNode : PHASESoundEventNode
    {
        /// <summary>
        /// Struct representing a random node entry that is use with the Sound Event Composer.
        /// </summary>
        [Serializable]
        public struct Entry
        {
            public string PortName;
            [Min(1)]
            public float Weight;
        }

        /// <summary>
        /// A <c>List</c> representing child nodes to randomly choose from.
        /// </summary>
        [SerializeField]
        public List<Entry> Entries = new List<Entry>();

        /// <summary>
        /// Create the random node in the PHASE engine.
        /// </summary>
        /// <returns>Returns true on success, false otherwise.</returns>
        public override bool Create()
        {
            // Create an array of entries that we can pass to the plugin.
            Helpers.RandomNodeEntry[] entries = new Helpers.RandomNodeEntry[Entries.Count];

            // Create all the nodes this random node contains.
            for (int entryIdx = 0; entryIdx < Entries.Count; ++entryIdx)
            {
                PHASESoundEventNode node = GetPort(Entries[entryIdx].PortName).GetConnection(0).node as PHASESoundEventNode;
                bool result = node.Create();
                if (result == false)
                {
                    Debug.LogError("Failed to create subnodes of random action tree node.");
                    return false;
                }

                // Store the node id and the weight.
                entries[entryIdx].NodeId = node.GetNodeId();
                entries[entryIdx].Weight = Entries[entryIdx].Weight;
            }

            // Now create the random node with all the entries. 
            GCHandle gcEntries = GCHandle.Alloc(entries, GCHandleType.Pinned);
            m_nodeId = Helpers.PHASECreateSoundEventRandomNode(gcEntries.AddrOfPinnedObject(), (uint)entries.Length);
            if (m_nodeId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create PHASE random node.");
                return false;
            }
            gcEntries.Free();

            return true;
        }

        /// <summary>
        /// Destroy this random node from the PHASE engine.
        /// </summary>
        public override void DestroyFromPHASE()
        {
            // Destroy the nodes on all the entries in this random node.
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
        /// Return a <c>List</c> of all mixers associated with this random node.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this random node.</returns>
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
            Entries.Add(new Entry() { PortName = port.fieldName, Weight = 1 });
        }

        public void RemoveEntry(int index)
        {
            RemoveDynamicPort(Entries[index].PortName);
            Entries.RemoveAt(index);
        }
#endif
    }
}