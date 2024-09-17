using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    // Keep the UI within the device's reported safe area.
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = default;
        [SerializeField] private RectTransform _safeAreaTransform = default;

        private Rect _lastCanvasRect = default;
        private Rect _lastSafeArea = default;

        void Update()
        {
            if (_canvas.pixelRect != _lastCanvasRect ||
                Screen.safeArea != _lastSafeArea)
            {
                _lastCanvasRect = _canvas.pixelRect;
                _lastSafeArea = Screen.safeArea;

                _safeAreaTransform.anchorMin = new Vector2(
                    _lastSafeArea.xMin / _lastCanvasRect.width,
                    _lastSafeArea.yMin / _lastCanvasRect.height);

                _safeAreaTransform.anchorMax = new Vector2(
                    _lastSafeArea.xMax / _lastCanvasRect.width,
                    _lastSafeArea.yMax / _lastCanvasRect.height);
            }
        }
    }
}