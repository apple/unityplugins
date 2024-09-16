using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    // The Unity UI can sometimes be hard to navigate with the Apple TV remote control.
    // This script helps manage the focus and highlights the current control to ease navigation.
    // Note: It's also important to bind joystick button 14 as described here:
    // https://docs.unity3d.com/Manual/tvos-setting-up-application-navigation.html
    public class NavigationHelper : MonoBehaviour
    {
        [SerializeField] private Color _selectedColor = Color.blue;
        
        void Start()
        {
            SelectFirstInteractableControl(gameObject);
        }

        void Update()
        {
            EnsureInteractableControlIsSelected();
        }

        private void UpdateSelectedColor(Selectable selectable)
        {
            var colors = selectable.colors;
            colors.selectedColor = _selectedColor;
            selectable.colors = colors;
        }

        // Select the first active and interactable control child of the given parent.
        // This helps keyboard navigation work more reliably.
        // Also sets the selected color of all the visited controls
        private Selectable SelectFirstInteractableControl(GameObject parent)
        {
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            Selectable objectToSelect = null;

            var camera = Camera.main;
            if (camera != null)
            {
                var corners = new Vector3[4];
                foreach (var selectable in parent.GetComponentsInChildren<Selectable>())
                {
                    // Set the selected color so we can tell when any control is selected.
                    UpdateSelectedColor(selectable);

                    if (selectable.isActiveAndEnabled && selectable.IsInteractable())
                    {
                        var rectTransform = selectable.GetComponent<RectTransform>();
                        rectTransform.GetWorldCorners(corners);
                        for (int i = 0; i < 4; i++)
                        {
                            var position = camera.WorldToViewportPoint(corners[i]);

                            if (position.y > maxY)
                            {
                                maxY = position.y;
                                objectToSelect = selectable;
                            }
                            else if (position.y == maxY && position.x < minX)
                            {
                                minX = position.x;
                                objectToSelect = selectable;
                            }
                        }
                    }
                }
            }

            if (objectToSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(objectToSelect.gameObject);
            }

            return objectToSelect;
        }

        // This helps address the issues raised here:
        // https://forum.unity.com/threads/keyboard-navigation-doesnt-work-if-you-click-off-the-ui.421977/
        private Selectable _lastSelectedObject;
        void EnsureInteractableControlIsSelected()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                GameObject currentGameObject = eventSystem.currentSelectedGameObject;
                Selectable current = (currentGameObject != null) ? currentGameObject.GetComponent<Selectable>() : null;
                if (current != null && current.isActiveAndEnabled && current.IsInteractable())
                {
                    if (_lastSelectedObject != current)
                    {
                        _lastSelectedObject = current;
                        UpdateSelectedColor(_lastSelectedObject);
                    }                    
                }
                else if (_lastSelectedObject != null && _lastSelectedObject.isActiveAndEnabled && _lastSelectedObject.IsInteractable())
                {
                    eventSystem.SetSelectedGameObject(_lastSelectedObject.gameObject);
                }
                else
                {
                    _lastSelectedObject = SelectFirstInteractableControl(gameObject);
                }
            }
        }
    }
}
