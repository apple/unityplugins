//
//  AssetPackManifest.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

import BackgroundAssets

fileprivate import OSLog
fileprivate let logger = Logger(subsystem: "com.apple.backgroundassets.wrapper", category: "AssetPackManifest")

extension AssetPackManifest {
	
	@available(iOS 27, macOS 27, tvOS 27, visionOS 27, *)
	init?(_ cManifest: baw_assetpackmanifest) {
		guard let manifest = cManifest.opaque?.load(as: Self.self) else {
			return nil
		}
		self = manifest
	}
	
}

extension baw_assetpackmanifest {
	
	@available(iOS 27, macOS 27, tvOS 27, visionOS 27, *)
	init(_ manifest: AssetPackManifest) {
		let pointer = UnsafeMutablePointer<AssetPackManifest>.allocate(capacity: 1)
		pointer.initialize(to: manifest)
		self.init(opaque: pointer)
	}
	
}

@c
@implementation public func baw_assetpackmanifest_deinit(_ cManifest: baw_assetpackmanifest) {
	if #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) {
		cManifest.opaque?
			.assumingMemoryBound(to: AssetPackManifest.self)
			.deinitialize(count: 1)
			.deallocate()
	} else {
		let cAssetPacks = UnsafeMutableBufferPointer(start: cManifest.compat.assetpackv, count: cManifest.compat.assetpackc)
		for cAssetPack in cAssetPacks {
			cAssetPack.deinitialize()
		}
		cAssetPacks
			.deinitialize()
			.deallocate()
	}
}

@c
@implementation public func baw_assetpackmanifest_assetpack(
	_ cManifest: baw_assetpackmanifest,
	_ cID: UnsafePointer<CChar>?
) -> baw_assetpack {
	guard let cID else {
		return baw_assetpack()
	}
	if #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) {
		return AssetPackManifest(cManifest)?
			.assetPack(
				withID: String(cString: cID)
			)
			.flatMap(baw_assetpack.init(_:)) ?? baw_assetpack()
	} else {
		for cAssetPack in UnsafeBufferPointer(start: cManifest.compat.assetpackv, count: cManifest.compat.assetpackc) {
			guard let assetPack = cAssetPack.impl?.load(as: AssetPack.self) else {
				continue
			}
			if assetPack.id == String(cString: cID) {
				return baw_assetpack(assetPack) // Return a copy
			}
		}
		return baw_assetpack()
	}
}

@c @implementation
public func baw_assetpackmanifest_assetpackv(
	_ cManifest: baw_assetpackmanifest,
	_ countPointer: UnsafeMutablePointer<Int>?
) -> UnsafeMutablePointer<baw_assetpack>? {
	logger.log(level: .fault, "C manifest: \(String(describing: cManifest))")
	logger.log(level: .fault, "C manifest opaque: \(String(describing: cManifest.opaque))")
	logger.log(level: .fault, "Count pointer: \(String(describing: countPointer))")
	if #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) {
		guard let manifest = AssetPackManifest(cManifest) else {
			return nil
		}
		logger.log(level: .fault, "Manifest: \(manifest)")
		logger.log(level: .fault, "Count: \(manifest.assetPacks.count)")
		let buffer = UnsafeMutableBufferPointer<baw_assetpack>.allocate(capacity: manifest.assetPacks.count)
		countPointer?.pointee = buffer.initialize(fromContentsOf: manifest.assetPacks.map(baw_assetpack.init(_:)))
		return buffer.baseAddress
	} else {
		let buffer = UnsafeMutableBufferPointer<baw_assetpack>.allocate(capacity: cManifest.compat.assetpackc)
		countPointer?.pointee = buffer.initialize(
			fromContentsOf: UnsafeBufferPointer(
				start: cManifest.compat.assetpackv,
				count: cManifest.compat.assetpackc
			).compactMap { (cAssetPack) in
				return baw_assetpack(
					cAssetPack.impl.load(as: AssetPack.self)
				) // Return a copy
			}
		)
		return buffer.baseAddress
	}
}

@c
@implementation public func baw_assetpackmanifest_assetpackv_localized(
	_ cManifest: baw_assetpackmanifest,
	_ countPointer: UnsafeMutablePointer<Int>?
) -> UnsafeMutablePointer<baw_assetpack>? {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *), let manifest = AssetPackManifest(cManifest) else {
		return nil
	}
	let buffer = UnsafeMutableBufferPointer<baw_assetpack>.allocate(capacity: manifest.localizedAssetPacks.count)
	countPointer?.pointee = buffer.initialize(fromContentsOf: manifest.localizedAssetPacks.map(baw_assetpack.init(_:)))
	return buffer.baseAddress
}

@c
@implementation public func baw_assetpackmanifest_assetpackv_localized_lang(
	_ cManifest: baw_assetpackmanifest,
	_ cLanguage: baw_lang,
	_ countPointer: UnsafeMutablePointer<Int>?
) -> UnsafeMutablePointer<baw_assetpack>? {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *), let manifest = AssetPackManifest(cManifest), let language = Locale.Language(cLanguage) else {
		return nil
	}
	let assetPacks = manifest.localizedAssetPacks(for: language)
	let buffer = UnsafeMutableBufferPointer<baw_assetpack>.allocate(capacity: assetPacks.count)
	countPointer?.pointee = buffer.initialize(fromContentsOf: assetPacks.map(baw_assetpack.init(_:)))
	return buffer.baseAddress
}

@c
@implementation public func baw_assetpackmanifest_assetpackv_deinit(_ count: Int, _ cAssetPacks: UnsafeMutablePointer<baw_assetpack>?) {
	cAssetPacks?
		.deinitialize(count: count)
		.deallocate()
}

@c
@implementation public func baw_assetpackmanifest_lang_primary(_ cManifest: baw_assetpackmanifest) -> baw_lang {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *), let manifest = AssetPackManifest(cManifest) else {
		return baw_lang()
	}
	return baw_lang(manifest.primaryLanguage)
}

@c
@implementation public func baw_assetpackmanifest_lang_resolved(_ cManifest: baw_assetpackmanifest) -> baw_lang {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *), let manifest = AssetPackManifest(cManifest) else {
		return baw_lang()
	}
	return baw_lang(manifest.resolvedLanguage)
}

@c
@implementation public func baw_assetpackmanifest_langv(
	_ cManifest: baw_assetpackmanifest,
	_ countPointer: UnsafeMutablePointer<Int>?
) -> UnsafeMutablePointer<baw_lang>? {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *), let manifest = AssetPackManifest(cManifest) else {
		return nil
	}
	let buffer = UnsafeMutableBufferPointer<baw_lang>.allocate(capacity: manifest.availableLanguages.count)
	countPointer?.pointee = buffer.initialize(fromContentsOf: manifest.availableLanguages.map(baw_lang.init(_:)))
	return buffer.baseAddress
}

@c
@implementation public func baw_assetpackmanifest_langv_deinit(_ count: Int, _ cLanguages: UnsafeMutablePointer<baw_lang>?) {
	cLanguages?
		.deinitialize(count: count)
		.deallocate()
}
