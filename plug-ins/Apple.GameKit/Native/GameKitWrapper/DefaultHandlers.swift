//
//  DefaultHandlers.swift
//  GameKitWrapper
//

class DefaultNSExceptionHandler {
    static var _callback : NSExceptionCallback? = nil;

    static func set(callback : NSExceptionCallback?) {
        _callback = callback;
    }

    // Note: This method aborts execution of the current thread without unwinding. Callers must clean up Swift resources before calling or leaks may result.
    static func throwException(_ exception : NSException) -> Never {
        if let callback = _callback {
            callback(exception.passRetainedUnsafeMutablePointer());
        }

        fatalError("NSError callback not set");
    }
}

class DefaultNSErrorHandler {
    static var _callback : NSErrorCallback? = nil;

    static func set(callback : NSErrorCallback?) {
        _callback = callback;
    }

    // Note: This method aborts execution of the current thread without unwinding. Callers must clean up Swift resources before calling or leaks may result.
    static func throwError(_ error : NSError) -> Never {
        if let callback = _callback {
            callback(error.passRetainedUnsafeMutablePointer());
        }

        fatalError("NSError callback not set");
    }

    // Note: this method never returns so ensure all Swift resources are cleaned up before calling!
    static func throwApiUnavailableError() -> Never {
        throwError(NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion));
    }
}

@_cdecl("DefaultNSExceptionHandler_Set")
public func DefaultNSExceptionHandler_Set(callback : NSExceptionCallback?) {
    DefaultNSExceptionHandler.set(callback: callback);
}

@_cdecl("DefaultNSErrorHandler_Set")
public func DefaultNSErrorHandler_Set(callback : NSErrorCallback?) {
    DefaultNSErrorHandler.set(callback: callback);
}
