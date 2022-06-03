//
//  GeneralCallbackTypeDefinitions.swift
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

import Foundation

public typealias char_p = UnsafeMutablePointer<Int8>;
public typealias uchar_p = UnsafeMutablePointer<UInt8>;

/**
  The callback for when any GameKit error has occured.
 */
public typealias ErrorCallback = @convention(c) (GCWError) -> Void;

/**
  The callback for any generic success callback.
 */
public typealias SuccessCallback = @convention(c) () -> Void;
