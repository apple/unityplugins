using UnityEngine;
using Unity.Mathematics;
using Apple.visionOS.SpatialController;
using UnityEngine.Rendering;

public static class SpatialControllerUtils
{
    /// <summary>
    /// Applies the origin transform from an AccessoryAnchor to a GameObject's transform.
    /// Sets both position and rotation based on the anchor's originFromAnchorTransform.
    /// </summary>
    /// <param name="go">The GameObject whose transform will be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    public static void ApplyAccessoryAnchorTransform(GameObject go, AccessoryAnchor anchor)
    {
        ApplyAccessoryAnchorTransform(go.transform, anchor);
    }

    /// <summary>
    /// Applies the transform from an AccessoryAnchor to a GameObject's transform.
    /// Sets both position and rotation based on the anchor accessory origin's pose.
    /// </summary>
    /// <param name="go">The GameObject whose transform will be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
    /// <returns>True if the anchor has the specified pose available and can set it.</returns>
    public static bool ApplyAccessoryAnchorTransform(GameObject go, AccessoryAnchor anchor, ARKitCoordinateSpace.Correction correction)
    {
        return ApplyAccessoryAnchorTransform(go.transform, anchor, correction);
    }

    /// <summary>
    /// Applies the transform from an AccessoryAnchor to a GameObject's transform.
    /// Sets both position and rotation based on the anchor accessory location's pose.
    /// </summary>
    /// <param name="go">The GameObject whose transform will be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    /// <param name="location">Which accessory location of anchor.accessory.locations[].</param>
    /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
    /// <returns>True if the anchor has the specified pose available and can set it.</returns>
    public static bool ApplyAccessoryAnchorTransform(GameObject go, AccessoryAnchor anchor, Accessory.LocationName location, ARKitCoordinateSpace.Correction correction)
    {
        return ApplyAccessoryAnchorTransform(go.transform, anchor, location, correction);
    }

    /// <summary>
    /// Applies the transform from an AccessoryAnchor to a Transform component.
    /// Sets both position and rotation based on the anchor's originFromAnchorTransform.
    /// Equivalent to ApplyAccessoryAnchorTransform(transform, anchor, ARKitCoordinateSpace.Correction.None).
    /// </summary>
    /// <param name="transform">The Transform component to be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    public static void ApplyAccessoryAnchorTransform(Transform transform, AccessoryAnchor anchor)
    {
        Pose pose = anchor.originFromAnchorTransform;

        // Apply to transform
        transform.SetPositionAndRotation(pose.position, pose.rotation);
    }

    /// <summary>
    /// Applies the transform from an AccessoryAnchor to a Transform component.
    /// Sets both position and rotation based on the anchor accessory origin's pose.
    /// </summary>
    /// <param name="go">The GameObject whose transform will be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
    /// <returns>True if the anchor has the specified pose available and can set it.</returns>
    public static bool ApplyAccessoryAnchorTransform(Transform transform, AccessoryAnchor anchor, ARKitCoordinateSpace.Correction correction)
    {
        Pose pose;
        if (BindOptional(out pose, anchor.coordinateSpace(correction)))
        {
            // Apply to transform
            transform.SetPositionAndRotation(pose.position, pose.rotation);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Applies the transform from an AccessoryAnchor to a Transform component.
    /// Sets both position and rotation based on the anchor accessory locations's pose.
    /// </summary>
    /// <param name="go">The GameObject whose transform will be updated</param>
    /// <param name="anchor">The AccessoryAnchor containing the transform data to apply</param>
    /// <param name="location">Which accessory location of anchor.accessory.locations[].</param>
    /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
    /// <returns>True if the anchor has the specified pose available and can set it.</returns>
    public static bool ApplyAccessoryAnchorTransform(Transform transform, AccessoryAnchor anchor, Accessory.LocationName location, ARKitCoordinateSpace.Correction correction)
    {
        Pose pose;
        if (BindOptional(out pose, anchor.coordinateSpace(location, correction)))
        {
            // Apply to transform
            transform.SetPositionAndRotation(pose.position, pose.rotation);
            return true;
        }
        return false;
    }

    static bool BindOptional(out Pose pose, Pose? optionalPose)
    {
        if (!optionalPose.HasValue)
        {
            pose = new Pose(Vector3.zero, Quaternion.identity);
            return false;
        }
        pose = optionalPose.Value;
        return true;
    }

    /// <summary>
    /// Determines if the specified accessory is a left-handed controller.
    /// </summary>
    /// <param name="accessory">The Accessory to check for handedness</param>
    /// <returns>True if the accessory is left-handed, false otherwise</returns>
    public static bool IsLeftController(Accessory accessory)
    {
        return accessory.inherentChirality == AccessoryChirality.Left;
    }

    /// <summary>
    /// Determines if the specified accessory is a right-handed controller.
    /// </summary>
    /// <param name="accessory">The Accessory to check for handedness</param>
    /// <returns>True if the accessory is right-handed, false otherwise</returns>
    public static bool IsRightController(Accessory accessory)
    {
        return accessory.inherentChirality == AccessoryChirality.Right;
    }


    /// <summary>
    /// Calculates the optimal time value for predicting accessory anchor positions.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeValue"/> representing the predicted display time for anchor calculations.
    /// Returns either the next frame time (if valid and in the future) or the current time plus 
    /// a default latency offset.
    /// </returns>
    /// <remarks>
    /// This method determines the appropriate time to use when predicting accessory positions by:
    /// <list type="number">
    /// <item>Getting the current system time</item>
    /// <item>Attempting to get the predicted next frame time</item>
    /// <item>Using the next frame time if it's valid and occurs after the current time</item>
    /// <item>Otherwise, falling back to current time plus a default 22ms latency compensation</item>
    /// </list>
    /// The default frame latency of 22ms (0.022 seconds) represents typical display pipeline latency
    /// for maintaining smooth tracking predictions when frame timing information is unavailable.
    /// </remarks>
    public static TimeValue GetPredictAnchorTime(double timeLatencyInMilliseconds = 0.022)
    {  
        double DefaultFrameLatencySeconds = timeLatencyInMilliseconds;
        var timeNow = AccessoryTracking.GetCurrentTime();
        var timeNextFrame = AccessoryTracking.GetPredictedNextFrameTime();
        var timeDisplayed = timeNextFrame.isValid() && timeNextFrame >= timeNow ? timeNextFrame : timeNow + DefaultFrameLatencySeconds;
        return timeDisplayed;
    }

}
