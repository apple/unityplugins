namespace Apple.CoreHaptics
{
    public enum CHErrorCode
    {
        InvalidEnginePointer = -1,
        
        EngineNotRunning = -4805,
        EngineStartTimeout = -4808,
        InvalidEventType = -4821,
        InvalidParameterType = -4820,
        InvalidPatternData = -4813,
        InvalidPatternPlayer = -4812,
        InvalidAudioSession = -4815,
        MemoryError = -4899,
        NotSupported = -4809,
        OperationNotPermitted = -4806,
        ServerInitFailed = -4810,
        ServerInterrupted = -4811,
        BadEventEntry = -4830,
        BadParameterEntry = -4831,
        InvalidAudioResource = -4824,
        ResourceNotAvailable = -4825,
        InvalidEventDuration = -4823,
        InvalidEventTime = -4822,
        InvalidTime = -4840,
        InvalidPatternDictionary = -4814,
        UnknownError = -4898
    }
}