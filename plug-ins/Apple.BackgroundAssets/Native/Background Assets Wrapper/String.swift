//
//  String.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

private import Darwin

extension String {
	
	var cStringCopy: UnsafeMutablePointer<CChar> {
		let cString = UnsafeMutablePointer<CChar>.allocate(capacity: self.utf8CString.count)
		cString.initialize(from: Array(self.utf8CString), count: self.utf8CString.count)
		return cString
	}
	
}
