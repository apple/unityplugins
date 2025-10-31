//
//  General_CallbackTypeDefinitions.swift
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation

public typealias char_p = UnsafeMutablePointer<Int8>
public typealias uchar_p = UnsafeMutablePointer<UInt8>

/**
  The callback for when any SpatialController error has occured.
  The data pointed to by SCError.localizedDescription is only
  guarenteed to persist within the scope of the callback.
 */
public typealias ErrorCallback = @convention(c) (SCError) -> Void

/**
  The callback for any generic success callback.
 */
public typealias SuccessCallback = @convention(c) () -> Void
