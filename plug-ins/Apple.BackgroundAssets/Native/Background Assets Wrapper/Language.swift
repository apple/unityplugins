//
//  Language.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

import Foundation

extension baw_lang {
	
	init(_ language: Locale.Language?) {
		if let language {
			let pointer = UnsafeMutablePointer<Locale.Language>.allocate(capacity: 1)
			pointer.initialize(to: language)
			self.init(impl: pointer)
		} else {
			self.init(impl: nil)
		}
	}
	
	func deinitialize() {
		self.impl?
			.assumingMemoryBound(to: Locale.Language.self)
			.deinitialize(count: 1)
			.deallocate()
	}
	
}

extension Locale.Language {
	
	init?(_ cLanguage: baw_lang) {
		guard let language = cLanguage.impl?.load(as: Self.self) else {
			return nil
		}
		self = language
	}
	
}

@c @implementation
public func baw_lang_init(_ cID: UnsafePointer<CChar>?) -> baw_lang {
	return baw_lang(
		cID.map { (cID) in
			return Locale.Language(
				identifier: String(cString: cID)
			)
		}
	)
}

@c @implementation
public func baw_lang_deinit(_ cLanguage: baw_lang) {
	cLanguage.deinitialize()
}

@c @implementation
public func baw_lang_is_nonnull(_ cLanguage: baw_lang) -> CBool {
	return cLanguage.impl != nil
}

@c @implementation
public func baw_lang_id_min(_ cLanguage: baw_lang) -> UnsafeMutablePointer<CChar>? {
	return Locale.Language(cLanguage)?.minimalIdentifier.cStringCopy
}

@c @implementation
public func baw_lang_id_max(_ cLanguage: baw_lang) -> UnsafeMutablePointer<CChar>? {
	return Locale.Language(cLanguage)?.maximalIdentifier.cStringCopy
}

@c @implementation
public func baw_lang_id_deinit(_ cID: UnsafeMutablePointer<CChar>?) {
	guard let cID else {
		return
	}
	let count = strlen(cID) + 1
	cID
		.deinitialize(count: count)
		.deallocate()
}

@c @implementation
public func baw_lang_equivalent(_ firstCLanguage: baw_lang, _ secondCLanguage: baw_lang) -> CBool {
	guard let firstLanguage = Locale.Language(firstCLanguage), let secondLanguage = Locale.Language(secondCLanguage) else {
		return firstCLanguage.impl == secondCLanguage.impl
	}
	return firstLanguage.isEquivalent(to: secondLanguage)
}
