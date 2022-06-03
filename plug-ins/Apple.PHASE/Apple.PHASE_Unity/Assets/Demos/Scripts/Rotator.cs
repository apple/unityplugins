using UnityEngine;

public class Rotator : MonoBehaviour
{

    public bool Rotate = true;
    [Range(10f, 100f)]
    public float Speed = 1.0f;

    void Update()
    {
        if (Rotate)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * Speed);
        }
    }
}
