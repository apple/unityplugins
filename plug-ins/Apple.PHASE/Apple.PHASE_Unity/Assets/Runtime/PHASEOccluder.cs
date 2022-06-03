using UnityEngine;
using System.Runtime.InteropServices;

namespace Apple.PHASE
{
    /// <summary>
    /// Class representing an occluder in the PHASE engine.
    /// </summary>
    public class PHASEOccluder : MonoBehaviour
    {
        /// <summary>
        /// Material on this occluder.
        /// </summary>
        [SerializeField] private protected Helpers.MaterialPreset _material = Helpers.MaterialPreset.Cardboard;

        // Transform to use.
        private Transform _transform;

        // Source id to store.
        protected long _occluderId = Helpers.InvalidId;

        // Awake is called before the scene starts.
        void Awake()
        {
            // Store the transform object (transform itself gets updated).
            _transform = GetComponent<Transform>();

            // Attempt to get the mesh filter from this object.
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Helpers.MeshData meshData = Helpers.GetMeshData(meshFilter, _transform);

                // Pin the arrays so we can pass these as pointers to the plugin.
                GCHandle gcVertices = GCHandle.Alloc(meshData.Vertices, GCHandleType.Pinned);
                GCHandle gcNormals = GCHandle.Alloc(meshData.Normals, GCHandleType.Pinned);
                GCHandle gcIndices = GCHandle.Alloc(meshData.Indices, GCHandleType.Pinned);

                // Create a source.
                _occluderId = Helpers.PHASECreateOccluder(meshData.VertexCount, gcVertices.AddrOfPinnedObject(), gcNormals.AddrOfPinnedObject(), meshData.IndexCount, gcIndices.AddrOfPinnedObject());
                if (_occluderId == Helpers.InvalidId)
                {
                    Debug.LogError("Failed to create PHASE Occluder");
                }

                // Release the pinned data.
                gcVertices.Free();
                gcNormals.Free();
                gcIndices.Free();
            }
        }

        // Start is called before the first frame update.
        void Start()
        {
            string material_name = GetMaterialNameFromPreset(_material);

            bool result = Helpers.PHASECreateMaterialFromPreset(material_name, _material);
            if (result == false)
            {
                Debug.LogError("Failed to create PHASE material.");
            }

            result = Helpers.PHASESetOccluderMaterial(_occluderId, material_name);
            if (result == false)
            {
                Debug.LogError($"Failed to set PHASE Material: {material_name} on PHASE Occluder: {_occluderId}");
            }
        }

        string GetMaterialNameFromPreset(Helpers.MaterialPreset preset)
        {
            string presetName = "Cardboard";
            switch (preset)
            {
                case Helpers.MaterialPreset.Cardboard:
                    presetName = "Cardboard";
                    break;
                case Helpers.MaterialPreset.Glass:
                    presetName = "Glass";
                    break;
                case Helpers.MaterialPreset.Brick:
                    presetName = "Brick";
                    break;
                case Helpers.MaterialPreset.Concrete:
                    presetName = "Concrete";
                    break;
                case Helpers.MaterialPreset.Drywall:
                    presetName = "Drywall";
                    break;
                case Helpers.MaterialPreset.Wood:
                    presetName = "Wood";
                    break;
            }
            return presetName;
        }

        // Update is called once per frame.
        virtual protected void Update()
        {
            if (_transform != null)
            {
                Matrix4x4 phaseTransform = Helpers.GetPhaseTransform(_transform);
                bool result = Helpers.PHASESetOccluderTransform(_occluderId, phaseTransform);
                if (result == false)
                {
                    Debug.LogError("Failed to set transform on occluder " + _occluderId);
                }
            }
        }

        /// <summary>
        /// Destroy this occluder form the PHASE engine.
        /// </summary>
        public void DestroyFromPHASE()
        {
            Helpers.PHASEDestroyMaterial(GetMaterialNameFromPreset(_material));
            Helpers.PHASEDestroyOccluder(_occluderId);
            _occluderId = Helpers.InvalidId;
        }

        void OnDestroy()
        {
            DestroyFromPHASE();
        }

        /// <summary>
        /// Get the unique ID of this occluder.
        /// </summary>
        /// <returns> The unique ID of this occluder. </returns>
        public long GetOccluderId()
        {
            return _occluderId;
        }

        void OnEnable()
        {
            if (_occluderId == Helpers.InvalidId)
            {
                Awake();
                Start();
            }
        }

        void OnDisable()
        {
            DestroyFromPHASE();
        }
    }
}