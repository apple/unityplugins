using UnityEngine;
using Apple.PHASE;

public class FootstepTrigger : MonoBehaviour
{
    public PHASESource Source;
    public bool TriggerSwitch;
    public float Frequency = 200;
    private int _counter = 0;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _wetness = 0.5f;

    void Update()
    {
        ++_counter;
        if (_counter > Frequency)
        {
            Source.Play();

            if (TriggerSwitch == true)
            {
                Source.SetMetaParameterValue("Terrain", "Wood");
            }
            else
            {
                Source.SetMetaParameterValue("Terrain", "Gravel");
            }
            Source.SetMetaParameterValue("Wetness", _wetness);
            _counter = 0;
        }
    }
}
