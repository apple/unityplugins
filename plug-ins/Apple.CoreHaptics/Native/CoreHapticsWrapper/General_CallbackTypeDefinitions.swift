//
//  GeneralCallbackTypeDefinitions.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

public typealias int_p = UnsafeMutablePointer<Int>;
public typealias char_p = UnsafeMutablePointer<Int8>;
public typealias uchar_p = UnsafeMutablePointer<UInt8>;

/**
  The callback for when any error has occured.
 */
public typealias ErrorCallback = @convention(c) (CHWError) -> Void;
public typealias ErrorWithPointerCallback = @convention(c) (UnsafeRawPointer, CHWError) -> Void;

/**
  The callback for any generic success callback.
 */
public typealias SuccessCallback = @convention(c) () -> Void;
public typealias SuccessWithPointerCallback = @convention(c) (UnsafeRawPointer) -> Void;

