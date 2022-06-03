using UnityEngine;
using Apple.PHASE;

public class AmbienceBlender : MonoBehaviour
{
    [Range(0.0f, 50.0f)]
    [SerializeField] private float _crowdBlend = 0.0f;

    [SerializeField] private PHASESource _source = null;

    // Update is called once per frame
    void Update()
    {
        _source.SetMetaParameterValue("CrowdCheer", _crowdBlend);
    }
}
