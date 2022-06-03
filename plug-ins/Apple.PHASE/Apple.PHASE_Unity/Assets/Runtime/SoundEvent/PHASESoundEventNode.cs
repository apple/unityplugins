using XNode;
using System.Collections.Generic;
using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Parent class for all sound event nodes.
    /// </summary>
    public abstract class PHASESoundEventNode : Node
    {
        /// <summary>
        /// The parent node of this sound event node.
        /// </summary>
        /// <remarks> The first sound event node with no parent is considered the root node of a <c>PHASESoundEventNodeGraph</c>.</remarks>
        /// <see cref="PHASESoundEventNodeGraph"/>
        [Input] public PHASESoundEventNode ParentNode = null;

        // Node id
        protected long m_nodeId = Helpers.InvalidId;

        /// <summary>
        /// Returns the lower level node ID for this node.
        /// </summary>
        /// <returns> A long representing the lower level node ID for this node.</returns>
        public long GetNodeId() { return m_nodeId; }

        /// <summary>
        /// Abstract function to create a node (different nodes implement this specialization)
        /// </summary>
        /// <returns> Returns true on success, false otherwise. </returns>
        public abstract bool Create();

        /// <summary>
        /// Destroy this sound event from the PHASE engine.
        /// </summary>
        public virtual void DestroyFromPHASE()
        {
            Helpers.PHASEDestroySoundEventNode(m_nodeId);
            m_nodeId = Helpers.InvalidId;
        }

        /// <summary>
        /// Returns all mixers associated with this sound event.
        /// </summary>
        /// <returns> A <c>List</c> of mixers associated with this sound event. </returns>
        public abstract List<PHASEMixer> GetMixers();

        public override object GetValue(NodePort port)
        {
            return this;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (from.node == this && (from.node == to.node || !CheckConnections(from.GetConnections())))
            {
                to.Disconnect(from);
                Debug.LogError("Recursive SoundEvent connections not allowed.");
            }
        }

        private bool CheckConnections(IEnumerable<NodePort> ports)
        {
            foreach (var port in ports)
            {
                if (port.node == null)
                {
                    continue;
                }

                if (port.node == this)
                {
                    return false;
                }

                var targetPorts = port.node.Outputs;

                foreach (var targetPort in targetPorts)
                {
                    if (!CheckConnections(targetPort.GetConnections()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected PHASEMixer GetMixerFromPort()
        {
            var mixerPort = GetOutputPort("Mixer");
            if (mixerPort != null && mixerPort.Connection != null && mixerPort.Connection.node != null)
            {
                return mixerPort.Connection.node as PHASEMixer;
            }
            else
            {
                return null;
            }
        }

        protected PHASESoundEventParameter GetParameterFromPort()
        {
            var parameterPort = GetOutputPort("Parameter");
            if (parameterPort != null && parameterPort.Connection != null && parameterPort.Connection.node != null)
            {
                return parameterPort.Connection.node as PHASESoundEventParameter;
            }
            else
            {
                return null;
            }
        }

        void OnDestroy()
        {
            DestroyFromPHASE();
        }
    }
}
