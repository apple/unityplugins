//
//  AssetPack.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

import BackgroundAssets

extension baw_assetpack {
	
	init(_ assetPack: AssetPack) {
		let pointer = UnsafeMutablePointer<AssetPack>.allocate(capacity: 1)
		pointer.initialize(to: assetPack)
		self.init(impl: pointer)
	}
	
	func deinitialize() {
		self.impl?
			.assumingMemoryBound(to: AssetPack.self)
			.deinitialize(count: 1)
			.deallocate()
	}
	
}

extension AssetPack {
	
	init?(_ cAssetPack: baw_assetpack) {
		guard let assetPack = cAssetPack.impl?.load(as: Self.self) else {
			return nil
		}
		self = assetPack
	}
	
}

@c @implementation
public func baw_assetpack_deinit(_ cAssetPack: baw_assetpack) {
	cAssetPack.deinitialize()
}

@c @implementation
public func baw_assetpack_is_nonnull(_ cAssetPack: baw_assetpack) -> CBool {
	return cAssetPack.impl != nil
}

@c @implementation
public func baw_assetpack_id(_ cAssetPack: baw_assetpack) -> UnsafeMutablePointer<CChar>? {
	return AssetPack(cAssetPack)?.id.cStringCopy
}

@c @implementation
public func baw_assetpack_id_deinit(_ cID: UnsafeMutablePointer<CChar>?) {
	guard let cID else {
		return
	}
	let count = strlen(cID) + 1
	cID
		.deinitialize(count: count)
		.deallocate()
}

@c @implementation
public func baw_assetpack_downloadsize(_ cAssetPack: baw_assetpack) -> Int {
	return AssetPack(cAssetPack)?.downloadSize ?? 0
}

@c @implementation
public func baw_assetpack_version(_ cAssetPack: baw_assetpack) -> UInt16 {
	return (AssetPack(cAssetPack)?.version).map(UInt16.init(_:)) ?? 0
}

@c @implementation
public func baw_assetpack_lang(_ cAssetPack: baw_assetpack) -> baw_lang {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		return baw_lang()
	}
	return baw_lang(AssetPack(cAssetPack)?.language)
}

@c @implementation
public func baw_assetpack_userinfo(
	_ cAssetPack: baw_assetpack,
	_ userInfoPointer: UnsafeMutableRawPointer?,
	_ userInfoCount: UnsafeMutablePointer<Int>?
) {
	guard let userInfoPointer, let userInfoCount else {
		return
	}
	guard let userInfo = AssetPack(cAssetPack)?.userInfo else {
		userInfoCount.pointee = 0
		return
	}
	userInfoCount.pointee = min(userInfo.count, userInfoCount.pointee)
	userInfo.copyBytes(
		to: userInfoPointer.assumingMemoryBound(to: UInt8.self),
		count: userInfoCount.pointee
	)
}
