using UnityEngine;

[System.Serializable]
public class Note : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string notes = "";
    [SerializeField] private bool isEditable = false;

    public string Notes
    {
        get { return notes; }
        set { notes = value; }
    }
    
    public bool IsEditable
    {
        get { return isEditable; }
        set { isEditable = value; }
    }

    // Optional: Add a header in the inspector
    [Space(10)]
    [Header("GameObject Notes")]
    public bool showInInspector = true;
}
