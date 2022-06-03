using UnityEngine.SceneManagement;
using UnityEngine;

namespace Apple.Accessibility.Demo
{
    internal class AccessibilityOpenScene : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
