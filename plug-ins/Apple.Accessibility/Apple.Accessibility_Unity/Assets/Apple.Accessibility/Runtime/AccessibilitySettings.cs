using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Accessibility
{
    /// <summary>
    /// Constants that indicate the preferred size of your content.
    /// </summary>
    [Serializable]
    public enum ContentSizeCategory
    {
        Unspecified = 0,
        ExtraSmall = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
        ExtraLarge = 5,
        ExtraExtraLarge = 6,
        ExtraExtraExtraLarge = 7,

        AccessibilityMedium = 8,
        AccessibilityLarge = 9,
        AccessibilityExtraLarge = 10,
        AccessibilityExtraExtraLarge = 11,
        AccessibilityExtraExtraExtraLarge = 12,
    }

    public static class AccessibilitySettings
    {
        public delegate void PreferredContentSizeChangedHandler();
        /// <summary>
        /// A notification that posts when the user changes the preferred content size setting.
        /// </summary>
        public static PreferredContentSizeChangedHandler onPreferredContentSizeChanged;

        /// <summary>
        /// The font sizing option preferred by the user.
        /// Listening for changes for this setting by subscribing to `onPreferredContentSizeChanged`.
        /// </summary>
        public static ContentSizeCategory PreferredContentSizeCategory
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return (ContentSizeCategory)_UnityAX_UIAccessibilityPreferredContentSizeCategory();
#else
                return ContentSizeCategory.Unspecified;
#endif
            }
        }

        /// <summary>
        /// The current font sizing multiplier value preffered by the user.
        /// Listening for changes for this setting by subscribing to `onPreferredContentSizeChanged`.
        /// </summary>
        public static float PreferredContentSizeMultiplier
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityPreferredContentSizeMultiplier();
#else
                return 1;
#endif
            }
        }


        public delegate void IsVoiceOverRunningChangedHandler();
        /// <summary>
        /// A notification that gets posted when VoiceOver starts or stops.
        /// </summary>
        public static IsVoiceOverRunningChangedHandler onIsVoiceOverRunningChanged;

        /// <summary>
        /// A Boolean value that indicates whether VoiceOver is in an enabled state.
        /// </summary>
        public static bool IsVoiceOverRunning
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsVoiceOverRunning();
#else
                return false;
#endif
            }
        }


        public delegate void IsSwitchControlRunningChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Switch Control setting changes.
        /// </summary>
        public static IsSwitchControlRunningChangedHandler onIsSwitchControlRunningChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Switch Control setting is in an enabled state.
        /// </summary>
        public static bool IsSwitchControlRunning
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsSwitchControlRunning();
#else
                return false;
#endif
            }
        }


        public delegate void IsSpeakSelectionEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Speak Selection setting changes.
        /// </summary>
        public static IsSpeakSelectionEnabledChangedHandler onIsSpeakSelectionEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Speak Selection setting is in an enabled state.
        /// </summary>
        public static bool IsSpeakSelectionEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsSpeakSelectionEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsGuidedAccessEnabledChangedHandler();
        /// <summary>
        /// A notification that indicates when a Guided Access session starts or ends.
        /// </summary>
        public static IsGuidedAccessEnabledChangedHandler onIsGuidedAccessEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Guided Access setting is in an enabled state.
        /// </summary>
        public static bool IsGuidedAccessEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsGuidedAccessEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsMonoAudioEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when system audio changes from stereo to mono.
        /// </summary>
        public static IsMonoAudioEnabledChangedHandler onIsMonoAudioEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Mono Audio setting is in an enabled state.
        /// </summary>
        public static bool IsMonoAudioEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsMonoAudioEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsClosedCaptioningEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the setting for Closed Captions + SDH changes.
        /// </summary>
        public static IsClosedCaptioningEnabledChangedHandler onIsClosedCaptioningEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Closed Captions + SDH setting is in an enabled state.
        /// </summary>
        public static bool IsClosedCaptioningEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsClosedCaptioningEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsInvertColorsEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the settings for inverted colors change.
        /// </summary>
        public static IsInvertColorsEnabledChangedHandler onIsInvertColorsEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Classic Invert setting is in an enabled state.
        /// </summary>
        public static bool IsInvertColorsEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsInvertColorsEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsBoldTextEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Bold Text setting changes.
        /// </summary>
        public static IsBoldTextEnabledChangedHandler onIsBoldTextEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Bold Text setting is in an enabled state.
        /// </summary>
        public static bool IsBoldTextEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsBoldTextEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void ButtonShapesEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Button Shapes setting changes.
        /// </summary>
        public static ButtonShapesEnabledChangedHandler onButtonShapesEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Button Shapes setting is in an enabled state.
        /// </summary>
        public static bool IsButtonShapesEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityButtonShapesEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsGrayscaleEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Grayscale setting changes.
        /// </summary>
        public static IsGrayscaleEnabledChangedHandler onIsGrayscaleEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Color Filters and the Grayscale settings are in an enabled state.
        /// </summary>
        public static bool IsGrayscaleEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsGrayscaleEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsReduceTransparencyEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Reduce Transparency setting changes.
        /// </summary>
        public static IsReduceTransparencyEnabledChangedHandler onIsReduceTransparencyEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Reduce Transparency setting is in an enabled state.
        /// </summary>
        public static bool IsReduceTransparencyEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsReduceTransparencyEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsReduceMotionEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Reduce Motion setting changes.
        /// </summary>
        public static IsReduceMotionEnabledChangedHandler onIsReduceMotionEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Reduce Motion setting is in an enabled state.
        /// </summary>
        public static bool IsReduceMotionEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsReduceMotionEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void PrefersCrossFadeTransitionsEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Prefer Cross-Fade Transitions setting changes.
        /// </summary>
        public static PrefersCrossFadeTransitionsEnabledChangedHandler onPrefersCrossFadeTransitionsEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Reduce Motion and the Prefer Cross-Fade Transitions settings are in an enabled state.
        /// </summary>
        public static bool PrefersCrossFadeTransitions
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityPrefersCrossFadeTransitions();
#else
                return false;
#endif
            }
        }


        public delegate void IsVideoAutoplayEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Auto-Play Video Previews setting changes.
        /// </summary>
        public static IsVideoAutoplayEnabledChangedHandler onIsVideoAutoplayEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Auto-Play Video Previews setting is in an enabled state.
        /// </summary>
        public static bool IsVideoAutoplayEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsVideoAutoplayEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsIncreaseContrastEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Increase Contrast setting changes.
        /// </summary>
        public static IsIncreaseContrastEnabledChangedHandler onIncreaseContrastEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Increase Contrast setting is in an enabled state.
        /// </summary>
        public static bool IsIncreaseContrastEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityDarkerSystemColorsEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsSpeakScreenEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Speak Screen setting changes.
        /// </summary>
        public static IsSpeakScreenEnabledChangedHandler onIsSpeakScreenEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Speak Screen setting is in an enabled state.
        /// </summary>
        public static bool IsSpeakScreenEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsSpeakScreenEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void IsShakeToUndoEnabledChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system's Shake to Undo setting changes.
        /// </summary>
        public static IsShakeToUndoEnabledChangedHandler onIsShakeToUndoEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the Shake to Undo setting is in an enabled state.
        /// </summary>
        public static bool IsShakeToUndoEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsShakeToUndoEnabled();
#else
                return false;
#endif
            }
        }


        public delegate void ShouldDifferentiateWithoutColorChangedHandler();
        /// <summary>
        /// A notification that gets posted when the system’s Differentiate Without Color setting changes.
        /// </summary>
        public static ShouldDifferentiateWithoutColorChangedHandler onShouldDifferentiateWithoutColorChanged;
        /// <summary>
        /// A Boolean value that indicates whether the Differentiate Without Color setting is in an enabled state.
        /// </summary>
        public static bool ShouldDifferentiateWithoutColor
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityShouldDifferentiateWithoutColor();
#else
                return false;
#endif
            }
        }


        public delegate void IsOnOffSwitchLabelsEnabledChangedHandler();
        /// <summary>
        /// A notification that UIKit posts when the system’s On/Off Labels setting changes.
        /// </summary>
        public static IsOnOffSwitchLabelsEnabledChangedHandler onIsOnOffSwitchLabelsEnabledChanged;

        /// <summary>
        /// A Boolean value that indicates whether the On/Off Labels setting is in an enabled state.
        /// </summary>
        public static bool IsOnOffSwitchLabelsEnabled
        {
            get
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                return _UnityAX_UIAccessibilityIsOnOffSwitchLabelsEnabled();
#else
                return false;
#endif
            }
        }

#region Native Bridge

        private static bool __registered = false;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RegisterCallbacks()
        {
            if (!__registered)
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                _UnityAX_registerAccessibilityPreferredContentSizeCategoryDidChangeNotification(_UnityAX_UIAccessibilityPreferredContentSizeCategoryDidChangeNotification);
                _UnityAX_registerAccessibilityIsVoiceOverRunningDidChangeNotification(_UnityAX_UIAccessibilityIsVoiceOverRunningDidChangeNotification);
                _UnityAX_registerAccessibilityIsSwitchControlRunningDidChangeNotification(_UnityAX_UIAccessibilityIsSwitchControlRunningDidChangeNotification);
                _UnityAX_registerAccessibilityIsSpeakSelectionEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsSpeakSelectionEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsGuidedAccessEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsGuidedAccessEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsMonoAudioEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsMonoAudioEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsClosedCaptioningEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsClosedCaptioningEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsInvertColorsEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsInvertColorsEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsBoldTextEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsBoldTextEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityButtonShapesEnabledDidChangeNotification(_UnityAX_UIAccessibilityButtonShapesEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsGrayscaleEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsGrayscaleEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsReduceTransparencyEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsReduceTransparencyEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsReduceMotionEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsReduceMotionEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityPrefersCrossFadeTransitionsDidChangeNotification(_UnityAX_UIAccessibilityPrefersCrossFadeTransitionsDidChangeNotification);
                _UnityAX_registerAccessibilityIsVideoAutoplayEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsVideoAutoplayEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityDarkerSystemColorsEnabledDidChangeNotification(_UnityAX_UIAccessibilityDarkerSystemColorsEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsSpeakScreenEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsSpeakScreenEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityIsShakeToUndoEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsShakeToUndoEnabledDidChangeNotification);
                _UnityAX_registerAccessibilityShouldDifferentiateWithoutColorDidChangeNotification(_UnityAX_UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotification);
                _UnityAX_registerAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotification(_UnityAX_UIAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotification);
#endif
                __registered = true;
            }
        }


#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityPreferredContentSizeCategoryDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern int _UnityAX_UIAccessibilityPreferredContentSizeCategory();
        [DllImport("__Internal")] private static extern float _UnityAX_UIAccessibilityPreferredContentSizeMultiplier();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityPreferredContentSizeCategoryDidChangeNotification(UIAccessibilityPreferredContentSizeCategoryDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityPreferredContentSizeCategoryDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityPreferredContentSizeCategoryDidChangeNotification()
        {
            onPreferredContentSizeChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsVoiceOverRunningDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsVoiceOverRunning();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsVoiceOverRunningDidChangeNotification(UIAccessibilityIsVoiceOverRunningDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsVoiceOverRunningDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsVoiceOverRunningDidChangeNotification()
        {
            onIsVoiceOverRunningChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsSwitchControlRunningDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsSwitchControlRunning();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsSwitchControlRunningDidChangeNotification(UIAccessibilityIsSwitchControlRunningDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsSwitchControlRunningDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsSwitchControlRunningDidChangeNotification()
        {
            onIsSwitchControlRunningChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsSpeakSelectionEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsSpeakSelectionEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsSpeakSelectionEnabledDidChangeNotification(UIAccessibilityIsSpeakSelectionEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsSpeakSelectionEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsSpeakSelectionEnabledDidChangeNotification()
        {
            onIsSpeakSelectionEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsGuidedAccessEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsGuidedAccessEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsGuidedAccessEnabledDidChangeNotification(UIAccessibilityIsGuidedAccessEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsGuidedAccessEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsGuidedAccessEnabledDidChangeNotification()
        {
            onIsGuidedAccessEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsMonoAudioEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsMonoAudioEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsMonoAudioEnabledDidChangeNotification(UIAccessibilityIsMonoAudioEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsMonoAudioEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsMonoAudioEnabledDidChangeNotification()
        {
            onIsMonoAudioEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsClosedCaptioningEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsClosedCaptioningEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsClosedCaptioningEnabledDidChangeNotification(UIAccessibilityIsClosedCaptioningEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsClosedCaptioningEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsClosedCaptioningEnabledDidChangeNotification()
        {
            onIsClosedCaptioningEnabledChanged?.Invoke();
        }
#endif


#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsInvertColorsEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsInvertColorsEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsInvertColorsEnabledDidChangeNotification(UIAccessibilityIsInvertColorsEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsInvertColorsEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsInvertColorsEnabledDidChangeNotification()
        {
            onIsInvertColorsEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsBoldTextEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsBoldTextEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsBoldTextEnabledDidChangeNotification(UIAccessibilityIsBoldTextEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsBoldTextEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsBoldTextEnabledDidChangeNotification()
        {
            onIsBoldTextEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityButtonShapesEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityButtonShapesEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityButtonShapesEnabledDidChangeNotification(UIAccessibilityButtonShapesEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityButtonShapesEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityButtonShapesEnabledDidChangeNotification()
        {
            onButtonShapesEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsGrayscaleEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsGrayscaleEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsGrayscaleEnabledDidChangeNotification(UIAccessibilityIsGrayscaleEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsGrayscaleEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsGrayscaleEnabledDidChangeNotification()
        {
            onIsGrayscaleEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsReduceTransparencyEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsReduceTransparencyEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsReduceTransparencyEnabledDidChangeNotification(UIAccessibilityIsReduceTransparencyEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsReduceTransparencyEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsReduceTransparencyEnabledDidChangeNotification()
        {
            onIsReduceTransparencyEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsReduceMotionEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsReduceMotionEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsReduceMotionEnabledDidChangeNotification(UIAccessibilityIsReduceMotionEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsReduceMotionEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsReduceMotionEnabledDidChangeNotification()
        {
            onIsReduceMotionEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityPrefersCrossFadeTransitionsDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityPrefersCrossFadeTransitions();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityPrefersCrossFadeTransitionsDidChangeNotification(UIAccessibilityPrefersCrossFadeTransitionsDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityPrefersCrossFadeTransitionsDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityPrefersCrossFadeTransitionsDidChangeNotification()
        {
            onPrefersCrossFadeTransitionsEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsVideoAutoplayEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsVideoAutoplayEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsVideoAutoplayEnabledDidChangeNotification(UIAccessibilityIsVideoAutoplayEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsVideoAutoplayEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsVideoAutoplayEnabledDidChangeNotification()
        {
            onIsVideoAutoplayEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityDarkerSystemColorsEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityDarkerSystemColorsEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityDarkerSystemColorsEnabledDidChangeNotification(UIAccessibilityDarkerSystemColorsEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityDarkerSystemColorsEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityDarkerSystemColorsEnabledDidChangeNotification()
        {
            onIncreaseContrastEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsSpeakScreenEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsSpeakScreenEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsSpeakScreenEnabledDidChangeNotification(UIAccessibilityIsSpeakScreenEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsSpeakScreenEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsSpeakScreenEnabledDidChangeNotification()
        {
            onIsSpeakScreenEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsShakeToUndoEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsShakeToUndoEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsShakeToUndoEnabledDidChangeNotification(UIAccessibilityIsShakeToUndoEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsShakeToUndoEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsShakeToUndoEnabledDidChangeNotification()
        {
            onIsShakeToUndoEnabledChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityShouldDifferentiateWithoutColor();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityShouldDifferentiateWithoutColorDidChangeNotification(UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotification()
        {
            onShouldDifferentiateWithoutColorChanged?.Invoke();
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        private delegate void UIAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate();

        [DllImport("__Internal")] private static extern bool _UnityAX_UIAccessibilityIsOnOffSwitchLabelsEnabled();
        [DllImport("__Internal")] private static extern void _UnityAX_registerAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotification(UIAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate actionDelegate);

        [AOT.MonoPInvokeCallback(typeof(UIAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate))]
        private static void _UnityAX_UIAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotification()
        {
            onIsOnOffSwitchLabelsEnabledChanged?.Invoke();
        }
#endif

        #endregion
    }
}
