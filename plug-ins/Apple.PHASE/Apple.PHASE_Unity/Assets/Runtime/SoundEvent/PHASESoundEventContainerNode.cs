using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// A class representing a sound event container node in PHASE.
    /// </summary>
    public class PHASESoundEventContainerNode : PHASESoundEventNode
    {
        /// <summary>
        /// A <c>List</c> representing all child nodes activated by this container node.
        /// </summary>
        [SerializeField]
        public List<string> Nodes = new List<string>();

        private long[] _subtreeIds = { };


        /// <summary>
        /// Create the container node in the PHASE engine.
        /// </summary>
        /// <returns> True on success, false otherwise. </returns>
        public override bool Create()
        {
            if (Nodes.Count <= 0)
            {
                return false;
            }
            _subtreeIds = new long[Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                PHASESoundEventNode node = GetPort(Nodes[i]).GetConnection(0).node as PHASESoundEventNode;
                bool result = node.Create();
                if (result)
                {
                    _subtreeIds[i] = node.GetNodeId();
                }
                else
                {
                    Debug.LogError("Failed to create subnodes of PHASE Sound Event Container Node");
                    return false;
                }
            }

            m_nodeId = Helpers.PHASECreateSoundEventContainerNode(_subtreeIds, _subtreeIds.Length);
            if (m_nodeId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create PHASE Sound Event Container Node");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Destroy the container node in the PHASE engine.
        /// </summary>
        public override void DestroyFromPHASE()
        {
            if (Nodes.Count > 0)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    PHASESoundEventNode node = GetPort(Nodes[i]).GetConnection(0).node as PHASESoundEventNode;
                    node.DestroyFromPHASE();
                }
            }
            base.DestroyFromPHASE();
        }

        /// <summary>
        /// Returns a <c>List</c> of all mixers associated with this container node.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this container node.</returns>
        public override List<PHASEMixer> GetMixers()
        {
            List<PHASEMixer> mixers = new List<PHASEMixer>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                PHASESoundEventNode node = GetPort(Nodes[i]).GetConnection(0).node as PHASESoundEventNode;
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
            Nodes.Add(port.fieldName);
        }

        public void RemoveEntry(int index)
        {
            RemoveDynamicPort(Nodes[index]);
            Nodes.RemoveAt(index);
        }
#endif
    }
}