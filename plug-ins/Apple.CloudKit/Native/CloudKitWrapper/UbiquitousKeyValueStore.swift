//
//  UbiquitousKeyValueStore.swift
//  CloudKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CloudKit
import OSLog

let logger = Logger()

public typealias NSUbiquitousKeyValueStoreDidChangeExternallyNotificationCallback = @convention(c) (Int, InteropStructArray) -> Void;

public enum NSUbiquitousKeyValueStoreChangeReason : Int
{
    case serverChange = 1,
         initialSyncChange = 2,
         quotaViolationChange = 3,
         accountChange = 4
}

public class NSUbiquitousKeyValueStoreDidChangeExternallyObserver: NSObject {

    public var DidChangeExternally: NSUbiquitousKeyValueStoreDidChangeExternallyNotificationCallback? = nil

    @objc
    public func ubiquitousKeyValueStoreDidChange
    (
        _ notification:Notification
    )
    {
        logger.debug("NSUbiquitousKeyValueStore_DidChangeExternally")
        guard let userInfo = notification.userInfo else { return }
        guard let reasonForChangeKey = userInfo[NSUbiquitousKeyValueStoreChangeReasonKey] as? Int else { return }
        var reasonForChange: NSUbiquitousKeyValueStoreChangeReason = .serverChange
        switch reasonForChangeKey {
        case NSUbiquitousKeyValueStoreServerChange:
            break
        case NSUbiquitousKeyValueStoreInitialSyncChange:
            reasonForChange = .initialSyncChange
            break
        case NSUbiquitousKeyValueStoreQuotaViolationChange:
            reasonForChange = .quotaViolationChange
            break
        case NSUbiquitousKeyValueStoreAccountChange:
            reasonForChange = .accountChange
            break
        default:
            return
        }
        guard let changedKeys = userInfo[NSUbiquitousKeyValueStoreChangedKeysKey] as? [String] else { return }
        DidChangeExternally?(reasonForChange.rawValue, InteropStructArray(pointer: Unmanaged.passRetained(changedKeys as NSArray).toOpaque(), length: Int32(changedKeys.count)))
    }
}

private var _activeObserverForDidChangeExternallyNotification: NSUbiquitousKeyValueStoreDidChangeExternallyObserver?

@_cdecl("NSUbiquitousKeyValueStore_AddObserverForDidChangeExternallyNotification")
public func NSUbiquitousKeyValueStore_AddObserverForDidChangeExternallyNotification
(
    callback : @escaping NSUbiquitousKeyValueStoreDidChangeExternallyNotificationCallback
)
{
    logger.debug("NSUbiquitousKeyValueStore_AddObserverForDidChangeExternallyNotification")
    if (_activeObserverForDidChangeExternallyNotification == nil) {
        _activeObserverForDidChangeExternallyNotification = NSUbiquitousKeyValueStoreDidChangeExternallyObserver()
    }
    _activeObserverForDidChangeExternallyNotification?.DidChangeExternally = callback

    NotificationCenter.default.addObserver(_activeObserverForDidChangeExternallyNotification!,
                                           selector: #selector(_activeObserverForDidChangeExternallyNotification!.ubiquitousKeyValueStoreDidChange(_:)),
        name: NSUbiquitousKeyValueStore.didChangeExternallyNotification,
        object: NSUbiquitousKeyValueStore.default)
}

@_cdecl("NSUbiquitousKeyValueStore_RemoveObserverForDidChangeExternallyNotification")
public func NSUbiquitousKeyValueStore_RemoveObserverForDidChangeExternallyNotification
(
)
{
    logger.debug("NSUbiquitousKeyValueStore_RemoveObserverForDidChangeExternallyNotification")
    guard let activeObserverForDidChangeExternallyNotification = _activeObserverForDidChangeExternallyNotification else { return }
    NotificationCenter.default.removeObserver(activeObserverForDidChangeExternallyNotification)
}

@_cdecl("NSUbiquitousKeyValueStore_GetArray")
public func NSUbiquitousKeyValueStore_GetArray
(
    forKey aKey: char_p
) -> InteropStructArray
{
    let key = aKey.toString()
    logger.debug("NSUbiquitousKeyValueStore_GetArray(forKey: \"\(key)\")")
    guard let array = NSUbiquitousKeyValueStore.default.array(forKey: key) else { return InteropStructArray() }
    return InteropStructArray(pointer: Unmanaged.passRetained(array as NSArray).toOpaque(), length: Int32(array.count))
}

@_cdecl("NSUbiquitousKeyValueStore_GetBool")
public func NSUbiquitousKeyValueStore_GetBool
(
    forKey aKey: char_p
) -> Bool
{
    let key = aKey.toString()
    let value = NSUbiquitousKeyValueStore.default.bool(forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_GetBool(forKey: \"\(key)\") -> \(value)")
    return value
}

@_cdecl("NSUbiquitousKeyValueStore_GetData")
public func NSUbiquitousKeyValueStore_GetData
(
    forKey aKey: char_p
) -> InteropStructArray
{
    let key = aKey.toString()
    logger.debug("NSUbiquitousKeyValueStore_GetData(forKey: \"\(key)\")")
    guard let data = NSUbiquitousKeyValueStore.default.data(forKey: key) else { return InteropStructArray() }
    return InteropStructArray(pointer: data.toUCharP(), length: Int32(data.count));
}

//@_cdecl("NSUbiquitousKeyValueStore_GetDictionary")
//public func NSUbiquitousKeyValueStore_GetDictionary
//(
//    forKey aKey: char_p
//) -> InteropStructDictionary
//{
//    guard let dict = NSUbiquitousKeyValueStore.default.dictionary(forKey: aKey.toString()) else { return InteropStructDictionary() }
//    let keys = InteropStructArray(pointer: Unmanaged.passRetained(dict.keys as NSArray).toOpaque(), length: Int32(dict.keys.count)
//    let values = InteropStructArray(pointer: Unmanaged.passRetained(dict.values as NSArray).toOpaque(), length: Int32(dict.values.count)
//    return InteropStructDictionary(keys: keys, values: values)
//}

@_cdecl("NSUbiquitousKeyValueStore_GetDouble")
public func NSUbiquitousKeyValueStore_GetDouble
(
    forKey aKey: char_p
) -> Double
{
    let key = aKey.toString()
    let value = NSUbiquitousKeyValueStore.default.double(forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_GetDouble(forKey: \"\(key)\") -> \(value)")
    return value
}

@_cdecl("NSUbiquitousKeyValueStore_GetInt64")
public func NSUbiquitousKeyValueStore_GetInt64
(
    forKey aKey: char_p
) -> Int64
{
    let key = aKey.toString()
    let value = NSUbiquitousKeyValueStore.default.longLong(forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_GetInt64(forKey: \"\(key)\") -> \(value)")
    return value
}

//@_cdecl("NSUbiquitousKeyValueStore_GetObject")
//public func NSUbiquitousKeyValueStore_GetObect
//(
//    forKey aKey: char_p
//) -> Any?
//{
//    return NSUbiquitousKeyValueStore.default.object(forKey: aKey.toString())
//}

@_cdecl("NSUbiquitousKeyValueStore_GetString")
public func NSUbiquitousKeyValueStore_GetString
(
    forKey aKey: char_p
) -> char_p?
{
    let key = aKey.toString()
    let value = NSUbiquitousKeyValueStore.default.string(forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_GetString(forKey: \"\(key)\") -> \(value ?? "nil")")
    return value?.toCharPCopy()
}

@_cdecl("NSUbiquitousKeyValueStore_SetArray")
public func NSUbiquitousKeyValueStore_SetArray
(
    _ anArray: InteropStructArray,
    forKey aKey: char_p
)
{
    let key = aKey.toString()
    logger.debug("NSUbiquitousKeyValueStore_SetArray(forKey: \"\(key)\")")
    NSUbiquitousKeyValueStore.default.set(anArray.toArray(), forKey: key)
}

@_cdecl("NSUbiquitousKeyValueStore_SetBool")
public func NSUbiquitousKeyValueStore_SetBool
(
    _ value: Bool,
    forKey aKey: char_p
) -> Void
{
    let key = aKey.toString()
    NSUbiquitousKeyValueStore.default.set(value, forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_SetBool(\(value), forKey: \"\(key)\")")
}

@_cdecl("NSUbiquitousKeyValueStore_SetData")
public func NSUbiquitousKeyValueStore_SetData
(
    _ value: InteropStructArray,
    forKey aKey: char_p
)
{
    let key = aKey.toString()
    logger.debug("NSUbiquitousKeyValueStore_SetData(forKey: \"\(key)\")")
    NSUbiquitousKeyValueStore.default.set(value.toData(), forKey: key)
}

//@_cdecl("NSUbiquitousKeyValueStore_SetDictionary")
//public func NSUbiquitousKeyValueStore_SetDictionary
//(
//    _ aDictionary: [char_p : Any]?,
//    forKey aKey: char_p
//)
//{
//    NSUbiquitousKeyValueStore.default.set(aDictionary, forKey: aKey.toString())
//}

@_cdecl("NSUbiquitousKeyValueStore_SetDouble")
public func NSUbiquitousKeyValueStore_SetDouble
(
    _ value: Double,
    forKey aKey: char_p
) -> Void
{
    let key = aKey.toString()
    NSUbiquitousKeyValueStore.default.set(value, forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_SetDouble(\(value), forKey: \"\(key)\")")
}

@_cdecl("NSUbiquitousKeyValueStore_SetInt64")
public func NSUbiquitousKeyValueStore_SetInt64
(
    _ value: Int64,
    forKey aKey: char_p
) -> Void
{
    let key = aKey.toString()
    NSUbiquitousKeyValueStore.default.set(value, forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_SetInt64(\(value), forKey: \"\(key)\")")
}

//@_cdecl("NSUbiquitousKeyValueStore_SetObject")
//public func NSUbiquitousKeyValueStore_SetObject
//(
//    _ value: Any?,
//    forKey aKey: char_p
//) -> Void
//{
//    NSUbiquitousKeyValueStore.default.set(value, forKey: aKey.toString())
//}

@_cdecl("NSUbiquitousKeyValueStore_SetString")
public func NSUbiquitousKeyValueStore_SetString
(
    _ value: char_p?,
    forKey aKey: char_p
) -> Void
{
    let key = aKey.toString()
    let stringValue = value?.toString() ?? ""
    NSUbiquitousKeyValueStore.default.set(stringValue, forKey: key)
    logger.debug("NSUbiquitousKeyValueStore_SetString(\(stringValue), forKey: \"\(key)\")")
}

@_cdecl("NSUbiquitousKeyValueStore_Synchronize")
public func NSUbiquitousKeyValueStore_Synchronize
(
) -> Bool
{
    logger.debug("NSUbiquitousKeyValueStore_Synchronize")
    return NSUbiquitousKeyValueStore.default.synchronize()
}

@_cdecl("NSUbiquitousKeyValueStore_RemoveObject")
public func NSUbiquitousKeyValueStore_RemoveObject
(
    forKey aKey: char_p
) -> Void
{
    let key = aKey.toString()
    logger.debug("NSUbiquitousKeyValueStore_RemoveObject(forKey: \"\(key)\"")
    NSUbiquitousKeyValueStore.default.removeObject(forKey: key)
}
