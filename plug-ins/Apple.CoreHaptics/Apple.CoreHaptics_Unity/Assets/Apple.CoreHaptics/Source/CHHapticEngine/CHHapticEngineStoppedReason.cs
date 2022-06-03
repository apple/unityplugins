namespace Apple.CoreHaptics
{
    public enum CHHapticEngineStoppedReason
    {
        SystemError = 1,
        AudioSessionInterrupt = 1,
        ApplicationSuspended = 2,
        IdleTimeout = 3,
        NotifyWhenFinished = 4,
        EngineDestroyed = 5,
        GameControllerDisconnect = 6
    }
}