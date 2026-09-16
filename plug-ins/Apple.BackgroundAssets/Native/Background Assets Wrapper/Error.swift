//
//  Error.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

private import Foundation

extension baw_err {
	
	static var missing: Self {
		return Self(staticDescription: "The data are missing.")
	}
	
	static var operationUnavailable: Self {
		return Self(staticDescription: "The operation is unavailable.")
	}
	
	private init(staticDescription: StaticString) {
		self.init(
			description: baw_err_description(str_static: staticDescription.utf8Start),
			_static: true
		)
	}
	
	init(_ error: some Error) {
		self.init(
			description: baw_err_description(str: error.localizedDescription.cStringCopy),
			_static: false
		)
	}
	
}

@c @implementation
public func baw_err_deinit(_ cError: baw_err) {
	if !cError._static, let cDescription = cError.description.str {
		let count = strlen(cDescription) + 1
		cDescription
			.deinitialize(count: count)
			.deallocate()
	}
}
