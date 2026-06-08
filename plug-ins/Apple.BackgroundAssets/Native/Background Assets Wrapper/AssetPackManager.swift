//
//  AssetPackManager.swift
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 3/26/26.
//  Copyright © 2026 Apple. All rights reserved.
//

private import BackgroundAssets
private import Synchronization
private import System

fileprivate final class DownloadStatusUpdatesObserver: Sendable {
	
	struct ContextPointerWrapper: Sendable {
		
		nonisolated(unsafe) let contextPointer: UnsafeMutableRawPointer?
		
		init(_ contextPointer: UnsafeMutableRawPointer?) {
			self.contextPointer = contextPointer
		}
		
	}
	
	static let shared = DownloadStatusUpdatesObserver()
	
	let handlers = Mutex<[String?: [(@convention(c) (baw_assetpackmanager_downloadstatusupdate, UnsafeMutableRawPointer?) -> Void, ContextPointerWrapper)]]>([:])
	
	init() {
		Task {
			for await statusUpdate in await AssetPackManager.shared.statusUpdates {
				self.handlers.withLock { (handlers) in
					func callHandlers(
						for assetPack: AssetPack,
						kind cKind: baw_assetpackmanager_downloadstatusupdate_kind,
						with cPayload: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload
					) {
						for (handler, contextPointerWrapper) in handlers[nil] ?? [] + (handlers[assetPack.id] ?? []) {
							handler(
								baw_assetpackmanager_downloadstatusupdate(
									kind: cKind,
									assetpack: baw_assetpack(assetPack),
									payload: cPayload
								),
								contextPointerWrapper.contextPointer
							)
						}
					}
					
					switch statusUpdate {
					case .began(let assetPack):
						callHandlers(
							for: assetPack,
							kind: baw_assetpackmanager_downloadstatusupdate_kind_began,
							with: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload()
						)
					case .paused(let assetPack):
						callHandlers(
							for: assetPack,
							kind: baw_assetpackmanager_downloadstatusupdate_kind_paused,
							with: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload()
						)
					case .downloading(let assetPack, let progress):
						callHandlers(
							for: assetPack,
							kind: baw_assetpackmanager_downloadstatusupdate_kind_downloading,
							with: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload(progress: progress.fractionCompleted)
						)
					case .finished(let assetPack):
						callHandlers(
							for: assetPack,
							kind: baw_assetpackmanager_downloadstatusupdate_kind_finished,
							with: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload()
						)
					case .failed(let assetPack, let error):
						callHandlers(
							for: assetPack,
							kind: baw_assetpackmanager_downloadstatusupdate_kind_failed,
							with: baw_assetpackmanager_downloadstatusupdate.__Unnamed_union_payload(err: baw_err(error))
						)
					@unknown default:
						break
					}
				}
			}
		}
	}
	
}

@c @implementation
public func baw_assetpackmanager_downloadstatusupdates(
	_ cID: UnsafePointer<CChar>?,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ handler: (@convention(c) (baw_assetpackmanager_downloadstatusupdate, UnsafeMutableRawPointer?) -> Void)?
) {
	guard let handler else {
		return
	}
	
	let contextPointerWrapper = DownloadStatusUpdatesObserver.ContextPointerWrapper(contextPointer)
	DownloadStatusUpdatesObserver.shared.handlers.withLock { (handlers) in
		handlers[cID.map(String.init(cString:)), default: []].append((handler, contextPointerWrapper))
	}
}

@c @implementation
public func baw_assetpackmanager_manifest(
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_assetpackmanifest_res, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		guard let completionHandler else {
			return
		}
		do {
			if #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) {
				let cManifest: baw_assetpackmanifest
				do {
					cManifest = baw_assetpackmanifest(try await AssetPackManager.shared.manifest)
					completionHandler(
						baw_assetpackmanifest_res(
							success: cManifest
						),
						baw_res_kind_success,
						contextPointer
					)
				}
			} else {
				let assetPacks = Array(try await AssetPackManager.shared.allAssetPacks)
				let buffer = UnsafeMutableBufferPointer<baw_assetpack>.allocate(capacity: assetPacks.count)
				let initializedCount = buffer.initialize(fromContentsOf: assetPacks.map(baw_assetpack.init(_:)))
				assert(initializedCount == buffer.count)
				completionHandler(
					baw_assetpackmanifest_res(
						success: baw_assetpackmanifest(
							compat: baw_assetpackmanifest_compat(
								assetpackc: buffer.count,
								assetpackv: buffer.baseAddress
							)
						)
					),
					baw_res_kind_success,
					contextPointer
				)
			}
		} catch {
			completionHandler(
				baw_assetpackmanifest_res(
					failure: baw_err(error)
				),
				baw_res_kind_failure,
				contextPointer
			)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_status(
	_ cAssetPack: baw_assetpack,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_assetpack_status_res, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard let assetPack = AssetPack(cAssetPack) else {
		completionHandler?(
			baw_assetpack_status_res(success: 0),
			baw_res_kind_success,
			contextPointer
		)
		return
	}
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		guard let completionHandler else {
			return
		}
		do {
			let status = if #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *) {
				try await AssetPackManager.shared.status(relativeTo: assetPack)
			} else {
				try await AssetPackManager.shared.status(ofAssetPackWithID: assetPack.id)
			}
			completionHandler(
				baw_assetpack_status_res(success: UInt8(status.rawValue)),
				baw_res_kind_success,
				contextPointer
			)
		} catch {
			completionHandler(
				baw_assetpack_status_res(failure: baw_err(error)),
				baw_res_kind_failure,
				contextPointer
			)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_status_local(
	_ cID: UnsafePointer<CChar>?,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (UInt8, UnsafeMutableRawPointer?) -> Void)?
) {
	guard #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *), let cID else {
		completionHandler?(0, contextPointer)
		return
	}
	let id = String(cString: cID)
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		completionHandler?(
			UInt8(
				await AssetPackManager.shared.localStatus(ofAssetPackWithID: id).rawValue
			),
			contextPointer
		)
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_local(_ cID: UnsafePointer<CChar>?) -> CBool {
	guard #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *), let cID else {
		return false
	}
	return AssetPackManager.shared.assetPackIsAvailableLocally(
		withID: String(cString: cID)
	)
}

@c @implementation
public func baw_assetpackmanager_assetpack_local_ensure(
	_ cAssetPack: baw_assetpack,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_err, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard let assetPack = AssetPack(cAssetPack) else {
		completionHandler?(.missing, baw_res_kind_failure, contextPointer)
		return
	}
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			if #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *) {
				try await AssetPackManager.shared.ensureLocalAvailability(of: assetPack)
			} else {
				try await AssetPackManager.shared.ensureLocalAvailability(of: assetPack)
			}
		} catch {
			completionHandler?(baw_err(error), baw_res_kind_failure, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_local_ensure_update(
	_ cAssetPack: baw_assetpack,
	_ shouldUpdate: CBool,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_err, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *) else {
		completionHandler?(.operationUnavailable, baw_res_kind_failure, contextPointer)
		return
	}
	guard let assetPack = AssetPack(cAssetPack) else {
		completionHandler?(.missing, baw_res_kind_failure, contextPointer)
		return
	}
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			try await AssetPackManager.shared.ensureLocalAvailability(of: assetPack, requireLatestVersion: shouldUpdate)
		} catch {
			completionHandler?(baw_err(error), baw_res_kind_failure, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_local_ensure_updatev(
	_ cAssetPacksCount: Int,
	_ cAssetPacksPointer: UnsafePointer<baw_assetpack>?,
	_ shouldUpdate: CBool,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_err, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		completionHandler?(.operationUnavailable, baw_res_kind_failure, contextPointer)
		return
	}
	guard let cAssetPacksPointer else {
		completionHandler?(.missing, baw_res_kind_failure, contextPointer)
		return
	}
	let assetPacks = Set(UnsafeBufferPointer(start: cAssetPacksPointer, count: cAssetPacksCount).compactMap(AssetPack.init(_:)))
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			try await AssetPackManager.shared.ensureLocalAvailability(of: assetPacks, requireLatestVersions: shouldUpdate)
		} catch {
			completionHandler?(baw_err(error), baw_res_kind_failure, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_update(
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_assetpackmanager_assetpack_update_res, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			let (updatingIDs, removedIDs) = try await AssetPackManager.shared.checkForUpdates()
			let cUpdatingIDs = updatingIDs.map(\.cStringCopy)
			let cRemovedIDs = removedIDs.map(\.cStringCopy)
			defer {
				for cID in cUpdatingIDs + cRemovedIDs {
					let count = strlen(cID) + 1
					cID
						.deinitialize(count: count)
						.deallocate()
				}
			}
			withUnsafeTemporaryAllocation(of: UnsafePointer<CChar>?.self, capacity: cUpdatingIDs.count) { (cUpdatingIDsBuffer) in
				let cUpdatingIDsCount = cUpdatingIDsBuffer.initialize(fromContentsOf: cUpdatingIDs)
				withUnsafeTemporaryAllocation(of: UnsafePointer<CChar>?.self, capacity: cRemovedIDs.count) { (cRemovedIDsBuffer) in
					let cRemovedIDsCount = cRemovedIDsBuffer.initialize(fromContentsOf: cRemovedIDs)
					completionHandler?(
						baw_assetpackmanager_assetpack_update_res(
							success: baw_assetpackmanager_assetpack_update_res.__Unnamed_struct_success(
								updatingc: cUpdatingIDsCount,
								updatingv: cUpdatingIDsBuffer.baseAddress,
								removedc: cRemovedIDsCount,
								removedv: cRemovedIDsBuffer.baseAddress
							)
						),
						baw_res_kind_success,
						contextPointer
					)
				}
			}
		} catch {
			completionHandler?(
				baw_assetpackmanager_assetpack_update_res(failure: baw_err(error)),
				baw_res_kind_failure,
				contextPointer
			)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_assetpack_remove(
	_ cID: UnsafePointer<CChar>?,
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_err, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard let cID else {
		completionHandler?(.missing, baw_res_kind_failure, contextPointer)
		return
	}
	let id = String(cString: cID)
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			try await AssetPackManager.shared.remove(assetPackWithID: id)
		} catch {
			completionHandler?(baw_err(error), baw_res_kind_failure, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_lang_resolved() -> baw_lang {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		return baw_lang()
	}
	return baw_lang(AssetPackManager.shared.resolvedLanguage)
}

@c @implementation
public func baw_assetpackmanager_lang_resolved_set(_ cLanguage: baw_lang) {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		return
	}
	AssetPackManager.shared.resolvedLanguage = Locale.Language(cLanguage)
}

@c @implementation
public func baw_assetpackmanager_lang_local(
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (Int, UnsafePointer<baw_lang>?, UnsafeMutableRawPointer?) -> Void)?
) {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		completionHandler?(0, nil, contextPointer)
		return
	}
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		guard let completionHandler else {
			return
		}
		let cLanguages = await AssetPackManager.shared.locallyAvailableLanguages.map(baw_lang.init(_:))
		cLanguages.withUnsafeBufferPointer { (buffer) in
			completionHandler(buffer.count, buffer.baseAddress, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_lang_reconcile(
	_ contextPointer: UnsafeMutableRawPointer?,
	_ completionHandler: (@convention(c) (baw_err, baw_res_kind, UnsafeMutableRawPointer?) -> Void)?
) {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		completionHandler?(.operationUnavailable, baw_res_kind_failure, contextPointer)
		return
	}
	nonisolated(unsafe) let contextPointer = contextPointer
	Task {
		do {
			try await AssetPackManager.shared.reconcilePreferredLanguages()
		} catch {
			completionHandler?(baw_err(error), baw_res_kind_failure, contextPointer)
		}
	}
}

@c @implementation
public func baw_assetpackmanager_open(
	_ cPath: UnsafePointer<CChar>?,
	_ cID: UnsafePointer<CChar>?,
	_ cError: UnsafeMutablePointer<baw_err>?
) -> CInt {
	guard let cPath else {
		cError?.pointee = .missing
		return -1
	}
	do {
		return try AssetPackManager.shared.descriptor(
			for: FilePath(platformString: cPath),
			searchingInAssetPackWithID: cID.map(String.init(cString:))
		).rawValue
	} catch {
		cError?.pointee = baw_err(error)
		return -1
	}
}

@c @implementation
public func baw_assetpackmanager_open_lang(
	_ cPath: UnsafePointer<CChar>?,
	_ cLanguage: baw_lang,
	_ cError: UnsafeMutablePointer<baw_err>?
) -> CInt {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		cError?.pointee = .operationUnavailable
		return -1
	}
	guard let cPath, let language = Locale.Language(cLanguage) else {
		cError?.pointee = .missing
		return -1
	}
	do {
		return try AssetPackManager.shared.descriptor(
			for: FilePath(platformString: cPath),
			asLocalizedFor: language
		).rawValue
	} catch {
		cError?.pointee = baw_err(error)
		return -1
	}
}

@c @implementation
public func baw_assetpackmanager_url(
	_ cPath: UnsafePointer<CChar>?,
	_ cError: UnsafeMutablePointer<baw_err>?
) -> UnsafeMutablePointer<CChar>? {
	guard let cPath else {
		cError?.pointee = .missing
		return nil
	}
	do {
		return try AssetPackManager.shared.url(
			for: FilePath(platformString: cPath)
		).absoluteString.cStringCopy
	} catch {
		cError?.pointee = baw_err(error)
		return nil
	}
}

@c @implementation
public func baw_assetpackmanager_url_lang(
	_ cPath: UnsafePointer<CChar>?,
	_ cLanguage: baw_lang,
	_ cError: UnsafeMutablePointer<baw_err>?
) -> UnsafeMutablePointer<CChar>? {
	guard #available(iOS 27, macOS 27, tvOS 27, visionOS 27, *) else {
		cError?.pointee = .operationUnavailable
		return nil
	}
	guard let cPath, let language = Locale.Language(cLanguage) else {
		cError?.pointee = .missing
		return nil
	}
	do {
		return try AssetPackManager.shared.url(
			for: FilePath(platformString: cPath),
			asLocalizedFor: language
		).absoluteString.cStringCopy
	} catch {
		cError?.pointee = baw_err(error)
		return nil
	}
}

@c @implementation
public func baw_assetpackmanager_url_deinit(_ cURL: UnsafeMutablePointer<CChar>?) {
	guard let cURL else {
		return
	}
	let count = strlen(cURL) + 1
	cURL
		.deinitialize(count: count)
		.deallocate()
}
