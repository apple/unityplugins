using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// Class representing a graph of sound event nodes.
    /// </summary>
    /// <remarks> Use the Asset menu to create a new instance of this class. </remarks>
    [CreateAssetMenu(menuName = "Apple/PHASE/SoundEvent", fileName = "NewSoundEvent")]
    public class PHASESoundEventNodeGraph : NodeGraph
    {
        static List<PHASESoundEventNodeGraph> m_registeredSoundEvents = new List<PHASESoundEventNodeGraph>();

        [System.NonSerialized]
        private bool m_isRegistered = false;

        // Root node of this action tree
        [SerializeField] private PHASESoundEventNode m_rootNode = null;

        /// <summary>
        /// Register this sound event graph with the PHASE engine.
        /// </summary>
        public void Register()
        {
            m_rootNode = GetRootNode();
            if (m_rootNode == null)
            {
                Debug.LogError($"No root node for PHASESoundEventNodeGraph: {name}.");
            }

            bool result = m_rootNode.Create();
            if (result == false)
            {
                Debug.LogError($"Failed to register PHASE action tree root node {m_rootNode.name}.");
                return;
            }

            // Create action tree asset
            result = Helpers.PHASERegisterSoundEventAsset(name, m_rootNode.GetNodeId());
            if (result == false)
            {
                Debug.LogError($"Failed to register PHASE action tree asset: {name}.");
            }
            else
            {
                m_registeredSoundEvents.Add(this);
                m_isRegistered = true;
            }
        }

        private PHASESoundEventNode GetRootNode()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                PHASESoundEventNode node = nodes[i] as PHASESoundEventNode;
                NodePort nodePort = nodes[i].GetInputPort("ParentNode");
                if (!nodePort.IsConnected)
                {
                    return node;
                }
            }
            return null;
        }

        protected override void OnDestroy()
        {
            Unregister();
            m_registeredSoundEvents.Remove(this);
            base.OnDestroy();
        }

        /// <summary>
        /// Unregister this sound event graph from the PHASE engine.
        /// </summary>
        public void Unregister()
        {
            Helpers.PHASEUnregisterSoundEventAsset(name);
            m_rootNode.DestroyFromPHASE();
        }

        /// <summary>
        /// Unregisters all sound event graphs from the PHASE engine.
        /// </summary>
        static public void UnregisterAll()
        {
            foreach (PHASESoundEventNodeGraph tree in m_registeredSoundEvents)
            {
                tree.Unregister();
            }

            m_registeredSoundEvents.Clear();
        }

        /// <summary>
        /// Checks if this sound event graph is registered with the PHASE engine.
        /// </summary>
        /// <returns> Return true if it is registered, false otherwise. </returns>
        public bool IsRegistered()
        {
            return m_isRegistered;
        }

        /// <summary>
        /// Returns a <c>List</c> of all mixers associated with this sound event graph.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this sound event graph.</returns>
        public List<PHASEMixer> GetMixers()
        {
            GetRootNode();
            if (m_rootNode != null)
            {
                return m_rootNode.GetMixers();
            }
            else
            {
                return null;
            }
        }
    }
}