using Apple.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ACA = Apple.Core.Availability;

public class AvailabilityManager : MonoBehaviour
{
    void Start()
    {
        if (ACA.Available(RuntimeOperatingSystem.macOS, 12, 5))
        {
            Debug.Log("I can call macOS 12.5 API!");
        }
        else
        {
            Debug.Log("I can't call macOS 12.5 API. I should implement a fallback.");
        }

        if (ACA.Available(RuntimeOperatingSystem.macOS, 13, 2))
        {
            Debug.Log("I can call macOS 13.2 API!");
        }
        else
        {
            Debug.Log("I can't call macOS 13.2 API. I should implement a fallback.");
        }

        if (ACA.Available(RuntimeOperatingSystem.iOS, 14) || ACA.Available(RuntimeOperatingSystem.macOS, 12) || ACA.Available(RuntimeOperatingSystem.tvOS, 14))
        {
            Debug.Log("I can call API that is compatible with iOS 14.0, macOS 12.0, or tvOS 14.0!");
        }
        else
        {
            Debug.Log("I can't call API that is compatible with iOS 14.0, macOS 12.0, or tvOS 14.0. I should implement a fallback.");
        }
    }
}
