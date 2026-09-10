using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class NavigationComponent : MonoBehaviour
{
    private Stack<GameObject> navigationStack = new();
    public Button backButton;
    public GameObject mainMenu;

    void Start()
    {
        backButton?.onClick.AddListener(NavigateBack);
        backButton.interactable = false;
    }

    public void NavigateTo(GameObject to)
    {
        if (!navigationStack.TryPeek(out var from))
        {
            from = mainMenu;
            navigationStack.Push(from);
        }
        
        Debug.Log($"Navigating to {to.name} from {from.name}");
        from.SetActive(false);
        to.SetActive(true);
        navigationStack.Push(to);
        UpdateBackButton();
    }

    public void NavigateBack()
    {
        if (navigationStack.TryPop(out var from))
        {
            Debug.Log($"Navigating back from {from.name}");
            from.SetActive(false);
        }

        if (navigationStack.TryPeek(out var to))
        {
            Debug.Log($"Navigating back to {to.name}");
            to.SetActive(true);
        }

        UpdateBackButton();
    }

    private void UpdateBackButton()
    {
        Debug.Log($"Navigation stack count: {navigationStack.Count}");
        backButton.interactable = navigationStack.Count > 1;
    }

}
