using AOT;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Apple.BackgroundAssets {
	
	/// <summary>
	/// A class that manages asset packs.
	///
	/// The first time that your code refers to the manager, Background Assets considers that your app is opting into automatic system management of your asset packs.
	/// </summary>
	public static class AssetPackManager {
		
		#nullable enable
		
		enum baw_assetpackmanager_downloadstatusupdate_kind {
			baw_assetpackmanager_downloadstatusupdate_kind_began,
			baw_assetpackmanager_downloadstatusupdate_kind_paused,
			baw_assetpackmanager_downloadstatusupdate_kind_downloading,
			baw_assetpackmanager_downloadstatusupdate_kind_finished,
			baw_assetpackmanager_downloadstatusupdate_kind_failed
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct baw_assetpackmanager_downloadstatusupdate {
			
			[StructLayout(LayoutKind.Explicit)]
			internal struct __Unnamed_union_payload {
				
				[FieldOffset(0)]
				internal double progress;
				
				[FieldOffset(0)]
				internal Error.baw_err err;
				
			}
			
			internal baw_assetpackmanager_downloadstatusupdate_kind kind;
			
			internal AssetPack.baw_assetpack assetpack;
			
			internal __Unnamed_union_payload payload;
			
		}
		
		[StructLayout(LayoutKind.Explicit)]
		struct baw_assetpackmanager_assetpack_update_res {
			
			[StructLayout(LayoutKind.Sequential)]
			internal struct __Unnamed_struct_success {
				
				internal long updatingc;
				
				internal unsafe IntPtr* updatingv;
				
				internal long removedc;
				
				internal unsafe IntPtr* removedv;
				
			}
			
			[FieldOffset(0)]
			internal __Unnamed_struct_success success;
			
			[FieldOffset(0)]
			internal Error.baw_err failure;
			
		}
		
		/// <summary>The status of an asset-pack download.</summary>
		public abstract class DownloadStatusUpdate {
			
			/// <summary>A status update that indicates that the download began or resumed after being paused.</summary>
			public class Began : DownloadStatusUpdate {
				
				internal Began(AssetPack assetPack) : base(assetPack) { }
				
				public override string ToString() {
					return $"{this.AssetPack.GetId()}: Began";
				}
				
			}
			
			/// <summary>A status update that indicates that the download paused.</summary>
			public class Paused : DownloadStatusUpdate {
				
				internal Paused(AssetPack assetPack) : base(assetPack) { }
				
				public override string ToString() {
					return $"{this.AssetPack.GetId()}: Paused";
				}
				
			}
			
			/// <summary>A status update that indicates that the download is in progress.</summary>
			public class Downloading : DownloadStatusUpdate {
				
				/// <summary>The download progress, represented as a portion between 0 and 1 (inclusive).</summary>
				public readonly double Progress;
				
				internal Downloading(AssetPack assetPack, double progress) : base(assetPack) {
					this.Progress = progress;
				}
				
				public override string ToString() {
					return $"{this.AssetPack.GetId()}: Downloading ({this.Progress})";
				}
				
			}
			
			/// <summary>A status update that indicates that the download completed and that the asset pack is available locally.</summary>
			public class Finished : DownloadStatusUpdate {
				
				internal override bool IsTerminal {
					get {
						return true;
					}
				}
				
				internal Finished(AssetPack assetPack) : base(assetPack) { }
				
				public override string ToString() {
					return $"{this.AssetPack.GetId()}: Finished";
				}
				
			}
			
			/// <summary>A status update that indicates that the download failed.</summary>
			public class Failed : DownloadStatusUpdate {
				
				/// <summary>An error that indicates why the download failed.</summary>
				public readonly Error Error;
				
				internal override bool IsTerminal {
					get {
						return true;
					}
				}
				
				internal Failed(AssetPack assetPack, Error error) : base(assetPack) {
					this.Error = error;
				}
				
				public override string ToString() {
					return $"{this.AssetPack.GetId()}: Failed ({this.Error})";
				}
				
			}
			
			/// <summary>The asset pack to which this status update pertains.</summary>
			public readonly AssetPack AssetPack;
			
			internal virtual bool IsTerminal {
				get {
					return false;
				}
			 }
			
			DownloadStatusUpdate(AssetPack assetPack) {
				this.AssetPack = assetPack;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct DownloadStatusUpdatesContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object LockObject;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object? StatusUpdate;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object? AssetPackId;
			
			internal DownloadStatusUpdatesContext(string? assetPackId) {
				this.LockObject = new object();
				this.StatusUpdate = null;
				this.Semaphore = new SemaphoreSlim(0);
				this.AssetPackId = assetPackId;
			}
			
			internal object GetLockObject() {
				return this.LockObject;
			}
			
			internal DownloadStatusUpdate? GetStatusUpdate() {
				return (DownloadStatusUpdate?) this.StatusUpdate;
			}
			
			internal void SetStatusUpdate(DownloadStatusUpdate statusUpdate) {
				this.StatusUpdate = statusUpdate;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
			internal void ResetSemaphore() {
				this.Semaphore = new SemaphoreSlim(0);
			}
			
			internal string? GetAssetPackId() {
				return (string?) this.AssetPackId;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct ManifestContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Result;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static ManifestContext Create() {
				return new ManifestContext {
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Result<AssetPackManifest, Error> GetResult() {
				return (Result<AssetPackManifest, Error>) this.Result;
			}
			
			internal void SetResult(Result<AssetPackManifest, Error> result) {
				this.Result = result;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct AssetPackStatusContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Result;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static AssetPackStatusContext Create() {
				return new AssetPackStatusContext {
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Result<AssetPack.Status, Error> GetResult() {
				return (Result<AssetPack.Status, Error>) this.Result;
			}
			
			internal void SetResult(Result<AssetPack.Status, Error> result) {
				this.Result = result;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct LocalAssetPackStatusContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Status;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static LocalAssetPackStatusContext Create() {
				return new LocalAssetPackStatusContext {
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal AssetPack.Status GetStatus() {
				return (AssetPack.Status) this.Status;
			}
			
			internal void SetStatus(AssetPack.Status status) {
				this.Status = status;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct EnsureLocalAvailabilityContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object? Error;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static EnsureLocalAvailabilityContext Create() {
				return new EnsureLocalAvailabilityContext {
					Error = null,
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Error? GetError() {
				return (Error?) this.Error;
			}
			
			internal void SetError(Error error) {
				this.Error = error;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct CheckForUpdatesContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Result;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static CheckForUpdatesContext Create() {
				return new CheckForUpdatesContext {
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Result<(string[], string[]), Error> GetResult() {
				return (Result<(string[], string[]), Error>) this.Result;
			}
			
			internal void SetResult(Result<(string[], string[]), Error> result) {
				this.Result = result;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct RemoveAssetPackContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object? Error;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static RemoveAssetPackContext Create() {
				return new RemoveAssetPackContext {
					Error = null,
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Error? GetError() {
				return (Error?) this.Error;
			}
			
			internal void SetError(Error error) {
				this.Error = error;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct LocallyAvailableLanguagesContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Languages;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static LocallyAvailableLanguagesContext Create() {
				return new LocallyAvailableLanguagesContext {
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Language[] GetLanguages() {
				return (Language[]) this.Languages;
			}
			
			internal void SetLanguages(Language[] languages) {
				this.Languages = languages;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct ReconcilePreferredLanguagesContext {
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object? Error;
			
			[MarshalAs(UnmanagedType.IUnknown)]
			object Semaphore;
			
			internal static ReconcilePreferredLanguagesContext Create() {
				return new ReconcilePreferredLanguagesContext {
					Error = null,
					Semaphore = new SemaphoreSlim(0)
				};
			}
			
			internal Error? GetError() {
				return (Error?) this.Error;
			}
			
			internal void SetError(Error error) {
				this.Error = error;
			}
			
			internal SemaphoreSlim GetSemaphore() {
				return (SemaphoreSlim) this.Semaphore;
			}
			
		}
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_downloadstatusupdates(string? id, IntPtr ctx, FDownloadStatusUpdates cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_manifest(IntPtr ctx, FManifest cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_status(AssetPack.baw_assetpack assetpack, IntPtr ctx, FAssetPackStatus cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_status_local(string id, IntPtr ctx, FAssetPackStatusLocal cb);
		
		[DllImport(InteropUtility.DllName)]
		[return: MarshalAs(UnmanagedType.I1)]
		static extern bool baw_assetpackmanager_assetpack_local(string id);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_local_ensure(AssetPack.baw_assetpack assetpack, IntPtr ctx, FAssetPackLocalEnsure cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_local_ensure_update(AssetPack.baw_assetpack assetpack, bool update, IntPtr ctx, FAssetPackLocalEnsure cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_local_ensure_updatev(int assetpackc, AssetPack.baw_assetpack[] assetpackv, bool update, IntPtr ctx, FAssetPackLocalEnsure cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_update(IntPtr ctx, FAssetPackUpdate cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_assetpack_remove(string id, IntPtr ctx, FAssetPackRemove cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern Language.baw_lang baw_assetpackmanager_lang_resolved();
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_lang_resolved_set(Language.baw_lang lang);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_lang_local(IntPtr ctx, FLangLocal cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_lang_reconcile(IntPtr ctx, FLangReconcile cb);
		
		[DllImport(InteropUtility.DllName)]
		static extern int baw_assetpackmanager_open(string path, string? id, out Error.baw_err err);
		
		[DllImport(InteropUtility.DllName)]
		static extern int baw_assetpackmanager_open_lang(string path, Language.baw_lang lang, out Error.baw_err err);
		
		[DllImport(InteropUtility.DllName)]
		static extern IntPtr baw_assetpackmanager_url(string path, out Error.baw_err err);
		
		[DllImport(InteropUtility.DllName)]
		static extern IntPtr baw_assetpackmanager_url_lang(string path, Language.baw_lang lang, out Error.baw_err err);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanager_url_deinit(IntPtr url);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FDownloadStatusUpdates(baw_assetpackmanager_downloadstatusupdate downloadstatusupdate, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FManifest(AssetPackManifest.baw_assetpackmanifest_res res, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FAssetPackStatus(AssetPack.Status.baw_assetpack_status_res res, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FAssetPackStatusLocal(byte status, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FAssetPackLocalEnsure(Error.baw_err err, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FAssetPackUpdate(baw_assetpackmanager_assetpack_update_res res, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FAssetPackRemove(Error.baw_err err, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		unsafe delegate void FLangLocal(long langc, Language.baw_lang* langv, IntPtr ctx);
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void FLangReconcile(Error.baw_err err, Result<object, Error>.baw_res_kind res_kind, IntPtr ctx);
		
		/// <summary>Returns an asynchronous sequence of download-status updates for all asset packs.</summary>
		/// <returns>An asynchronous sequence of download-status updates.</returns>
		public static async IAsyncEnumerable<DownloadStatusUpdate> DownloadStatusUpdatesAsync() {
			DownloadStatusUpdatesContext context = new DownloadStatusUpdatesContext(null);
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_downloadstatusupdates(null, contextPointer, DownloadStatusUpdatesCallback);
			while (true) {
				await context.GetSemaphore().WaitAsync();
				lock (context.GetLockObject()) {
					context = Marshal.PtrToStructure<DownloadStatusUpdatesContext>(contextPointer);
					if (context.GetStatusUpdate() is DownloadStatusUpdate statusUpdate) {
						yield return statusUpdate;
					}
					context.ResetSemaphore();
					Marshal.StructureToPtr(context, contextPointer, true);
				}
			}
		}
		
		/// <summary>
		/// Returns an asynchronous sequence of download-status updates for the asset pack with the specified ID.
		///
		/// The sequence finishes after yielding an instance of DownloadStatusUpdate.Finished or DownloadStatusUpdate.Failed.
		/// </summary>
		/// <param name="assetPackId">The asset pack’s ID.</param>
		/// <returns>An asynchronous sequence of download-status updates.</returns>
		public static async IAsyncEnumerable<DownloadStatusUpdate> DownloadStatusUpdatesAsync(string assetPackId) {
			DownloadStatusUpdatesContext context = new DownloadStatusUpdatesContext(assetPackId);
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_downloadstatusupdates(assetPackId, contextPointer, DownloadStatusUpdatesCallback);
			while (true) {
				await context.GetSemaphore().WaitAsync();
				bool shouldBreak = false;
				lock (context.GetLockObject()) {
					context = Marshal.PtrToStructure<DownloadStatusUpdatesContext>(contextPointer);
					if (context.GetStatusUpdate() is DownloadStatusUpdate statusUpdate) {
						yield return statusUpdate;
						if (statusUpdate.IsTerminal) {
							shouldBreak = true;
						}
					}
					context.ResetSemaphore();
					Marshal.StructureToPtr(context, contextPointer, true);
				}
				if (shouldBreak) {
					break;
				}
			}
		}
		
		[MonoPInvokeCallback(typeof(FDownloadStatusUpdates))]
		static void DownloadStatusUpdatesCallback(baw_assetpackmanager_downloadstatusupdate cStatusUpdate, IntPtr contextPointer) {
			DownloadStatusUpdatesContext context = Marshal.PtrToStructure<DownloadStatusUpdatesContext>(contextPointer);
			AssetPack assetPack = new AssetPack(cStatusUpdate.assetpack);
			if (context.GetAssetPackId() is string assetPackId) {
				if (assetPack.GetId() != assetPackId) {
					return; // Filter out status updates for other asset packs
				}
			}
			lock (context.GetLockObject()) {
				switch (cStatusUpdate.kind) {
				case baw_assetpackmanager_downloadstatusupdate_kind.baw_assetpackmanager_downloadstatusupdate_kind_began:
					context.SetStatusUpdate(new DownloadStatusUpdate.Began(assetPack));
					break;
				case baw_assetpackmanager_downloadstatusupdate_kind.baw_assetpackmanager_downloadstatusupdate_kind_paused:
					context.SetStatusUpdate(new DownloadStatusUpdate.Paused(assetPack));
					break;
				case baw_assetpackmanager_downloadstatusupdate_kind.baw_assetpackmanager_downloadstatusupdate_kind_downloading:
					context.SetStatusUpdate(new DownloadStatusUpdate.Downloading(assetPack, cStatusUpdate.payload.progress));
					break;
				case baw_assetpackmanager_downloadstatusupdate_kind.baw_assetpackmanager_downloadstatusupdate_kind_finished:
					context.SetStatusUpdate(new DownloadStatusUpdate.Finished(assetPack));
					break;
				case baw_assetpackmanager_downloadstatusupdate_kind.baw_assetpackmanager_downloadstatusupdate_kind_failed:
					context.SetStatusUpdate(new DownloadStatusUpdate.Failed(assetPack, new Error(cStatusUpdate.payload.err)));
					break;
				}
				Marshal.StructureToPtr(context, contextPointer, true);
				context.GetSemaphore().Release();
			}
		}
		
		/// <summary>Returns the manifest of asset packs that are available to download.</summary>
		/// <returns>A task that yields the manifest.</returns>
		public static async Task<AssetPackManifest> GetManifestAsync() {
			ManifestContext context = ManifestContext.Create();
			int contextSize = Marshal.SizeOf<ManifestContext>();
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_manifest(contextPointer, ManifestCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<ManifestContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			return context.GetResult().Get();
		}
		
		[MonoPInvokeCallback(typeof(FManifest))]
		static void ManifestCallback(AssetPackManifest.baw_assetpackmanifest_res cResult, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			ManifestContext context = Marshal.PtrToStructure<ManifestContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				context.SetResult(new Result<AssetPackManifest, Error>(new AssetPackManifest(cResult.success)));
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetResult(new Result<AssetPackManifest, Error>(new Error(cResult.failure)));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// Checks the current status relative to a particular asset pack.
		///
		/// This method checks whether any version of the specified asset pack is currently downloaded. If one is, then it determines the version relationship between the downloaded asset pack and the specified asset pack. If they have different version numbers, then the returned status object will contain “out of date”. The returned status object will contain “update available” only if the relevant asset pack on the server hasn’t been further updated since the initialization of the provided AssetPack object.
		///
		/// For example, consider the following sequence of events, assuming that version 1 of the relevant asset pack is already available locally:
		/// 1.	Your app calls GetAssetPack(string) on the AssetPackManifest object that GetManifestAsync() returns to obtain an AssetPack object.
		/// 2.	The asset pack is updated to version 2 on the server.
		/// 3.	Your app calls this method, passing the AssetPack object from step 1.
		/// In this case, the returned status object will indicate that the downloaded asset pack is up to date. Generally, you shouldn’t need to handle this type of situation explicitly because the system automatically polls for updates periodically in the background.
		///
		/// This method doesn’t automatically trigger any downloads, updates, or removals.
		/// </summary>
		/// <param name="assetPack">The asset pack.</param>
		/// <returns>A task that yields the status object.</returns>
		public static async Task<AssetPack.Status> GetAssetPackStatusAsync(AssetPack assetPack) {
			AssetPackStatusContext context = AssetPackStatusContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_status(assetPack.CAssetPack, contextPointer, AssetPackStatusCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<AssetPackStatusContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			return context.GetResult().Get();
		}
		
		[MonoPInvokeCallback(typeof(FAssetPackStatus))]
		static void AssetPackStatusCallback(AssetPack.Status.baw_assetpack_status_res cResult, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			AssetPackStatusContext context = Marshal.PtrToStructure<AssetPackStatusContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				context.SetResult(new Result<AssetPack.Status, Error>(new AssetPack.Status(cResult.success)));
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetResult(new Result<AssetPack.Status, Error>(new Error(cResult.failure)));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// Checks an asset pack’s local status.
		///
		/// This method checks only status values that are determinable offline. It doesn’t induce any network traffic or automatically trigger any downloads, updates, or removals. The following status values are determinable offline:
		/// - “Out of date” (in some situations)
		/// - “Obsolete” (in some situations)
		/// - “Downloaded”
		///
		/// Because this method doesn’t communicate with the server, it can’t determine whether a particular asset pack exists in the first place. Instead, it returns an empty status object when provided a nonexistent asset-pack ID, which is indistinguishable from the situation in which the asset pack does indeed exist but hasn’t yet been downloaded. Use GetAssetPackStatusAsync(AssetPack) to get a full view of an asset pack’s status.
		/// </summary>
		/// <param name="assetPackId">The asset pack’s ID.</param>
		/// <returns>
		/// A task that yields the status object.
		/// </returns>
		public static async Task<AssetPack.Status> GetLocalAssetPackStatusAsync(string id) {
			LocalAssetPackStatusContext context = LocalAssetPackStatusContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_status_local(id, contextPointer, LocalAssetPackStatusCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<LocalAssetPackStatusContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			return context.GetStatus();
		}
		
		[MonoPInvokeCallback(typeof(FAssetPackStatusLocal))]
		static void LocalAssetPackStatusCallback(byte cStatus, IntPtr contextPointer) {
			LocalAssetPackStatusContext context = Marshal.PtrToStructure<LocalAssetPackStatusContext>(contextPointer);
			context.SetStatus(new AssetPack.Status(cStatus));
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>Checks whether an asset pack is available locally.</summary>
		/// <param name="assetPackId">The asset pack’s ID.</param>
		/// <returns>Whether the asset pack is available locally.</returns>
		public static bool AssetPackIsAvailableLocally(string id) {
			return baw_assetpackmanager_assetpack_local(id);
		}
		
		/// <summary>
		/// Ensures that the specified asset pack be available locally.
		///
		/// This method checks whether the asset pack is currently downloaded, ignoring available updates. If it isn’t, then the system schedules it to be downloaded and waits for the download to finish. It’s guaranteed that the requested asset pack will be available locally once this method returns without throwing. If the method throws, then the asset pack is not guaranteed to be available locally. You can optionally monitor download progress by awaiting status updates from DownloadStatusUpdatesAsync() or DownloadStatusUpdatesAsync(string) in a separate task.
		/// </summary>
		/// <param name="assetPack">The asset pack the local availability of which to ensure.</param>
		/// <returns>A task that yields when the asset pack is available locally</returns>
		public static async Task EnsureLocalAvailabilityOfAssetPackAsync(AssetPack assetPack) {
			EnsureLocalAvailabilityContext context = EnsureLocalAvailabilityContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_local_ensure(assetPack.CAssetPack, contextPointer, EnsureLocalAvailabilityCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<EnsureLocalAvailabilityContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			if (context.GetError() is Error error) {
				throw error;
			}
		}
		
		/// <summary>
		/// Ensures that the specified asset pack be available locally.
		///
		/// This method checks whether the asset pack is currently downloaded. If it isn’t, then the system schedules it to be downloaded and waits for the download to finish. It’s guaranteed that the requested asset pack will be available locally once this method returns without throwing. If the method throws, then the asset pack is not guaranteed to be available locally. You can optionally monitor download progress by awaiting status updates from DownloadStatusUpdatesAsync() or DownloadStatusUpdatesAsync(string) in a separate task.
		/// </summary>
		/// <param name="assetPack">The asset pack the local availability of which to ensure.</param>
		/// <param name="shouldUpdate">Whether to require that the latest version be available locally. When true is passed to this parameter, the method will wait for the update (if there indeed is one available) to be downloaded before returning. When false is passed, the method won’t check for updates and won’t attempt to download any.</param>
		/// <returns>A task that yields when the asset pack is available locally.</returns>
		public static async Task EnsureLocalAvailabilityOfAssetPackAsync(AssetPack assetPack, bool shouldUpdate) {
			EnsureLocalAvailabilityContext context = EnsureLocalAvailabilityContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_local_ensure_update(assetPack.CAssetPack, shouldUpdate, contextPointer, EnsureLocalAvailabilityCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<EnsureLocalAvailabilityContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			if (context.GetError() is Error error) {
				throw error;
			}
		}
		
		/// <summary>
		/// Ensures that the specified asset packs be available locally.
		///
		/// This method checks whether the asset packs are currently downloaded. If any aren’t, then the system schedules them to be downloaded and waits for all of the downloads to finish. It’s guaranteed that the requested asset packs will be available locally once this method returns without throwing. If the method throws, then the asset packs are not all guaranteed to be available locally, though some might be; inspect the thrown error for more details. You can optionally monitor download progress by awaiting status updates from DownloadStatusUpdatesAsync() or DownloadStatusUpdatesAsync(string) in a separate task.
		/// </summary>
		/// <param name="assetPacks">The asset packs the local availability of which to ensure.</param>
		/// <param name="shouldUpdate"Whether to require that the respective latest versions be available locally. When true is passed to this parameter, the method will wait for the updates (if there indeed are any available) to be downloaded before returning. When false is passed, the method won’t check for updates and won’t attempt to download any.</param>
		/// <returns>A task that yields when the asset packs are available locally.</returns>
		public static async void EnsureLocalAvailabilityOfAssetPacksAsync(AssetPack[] assetPacks, bool shouldUpdate) {
			EnsureLocalAvailabilityContext context = EnsureLocalAvailabilityContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			AssetPack.baw_assetpack[] cAssetPacks = assetPacks.Select(assetPack => assetPack.CAssetPack).ToArray();
			baw_assetpackmanager_assetpack_local_ensure_updatev(cAssetPacks.Count(), cAssetPacks, shouldUpdate, contextPointer, EnsureLocalAvailabilityCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<EnsureLocalAvailabilityContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			if (context.GetError() is Error error) {
				throw error;
			}
		}
		
		[MonoPInvokeCallback(typeof(FAssetPackLocalEnsure))]
		static void EnsureLocalAvailabilityCallback(Error.baw_err cError, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			EnsureLocalAvailabilityContext context = Marshal.PtrToStructure<EnsureLocalAvailabilityContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetError(new Error(cError));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// Gets the latest asset-pack information from the server, updates outdated asset packs, and removes obsolete asset packs.
		///
		/// This method waits for any downloads that it schedules to be registered with the download manager, but it doesn’t wait for those downloads to begin or to finish. If you want to monitor download progress, then you should await status updates from DownloadStatusUpdatesAsync() or DownloadStatusUpdatesAsync(string).
		/// </summary>
		/// <returns>A task the yields a 2-tuple with the set of IDs of asset packs that are being updated and the set of IDs of asset packs that were removed as a result of the check for updates. Neither updates nor removals that weren’t triggered by the check for updates are taken into account.</returns>
		public static async Task<(string[] UpdatingIds, string[] RemovedIds)> CheckForUpdatesAsync() {
			CheckForUpdatesContext context = CheckForUpdatesContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_update(contextPointer, CheckForUpdatesCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<CheckForUpdatesContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			return context.GetResult().Get();
		}
		
		[MonoPInvokeCallback(typeof(FAssetPackUpdate))]
		static void CheckForUpdatesCallback(baw_assetpackmanager_assetpack_update_res cResult, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			CheckForUpdatesContext context = Marshal.PtrToStructure<CheckForUpdatesContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				unsafe {
					string[] updatingIds = new string[cResult.success.updatingc];
					for (long index = 0; index < cResult.success.updatingc; ++index) {
						updatingIds[index] = Marshal.PtrToStringUTF8(cResult.success.updatingv[index]);
					}
					string[] removedIds = new string[cResult.success.removedc];
					for (long index = 0; index < cResult.success.removedc; ++index) {
						removedIds[index] = Marshal.PtrToStringUTF8(cResult.success.removedv[index]);
					}
					context.SetResult(new Result<(string[], string[]), Error>((updatingIds, removedIds)));
				}
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetResult(new Result<(string[], string[]), Error>(new Error(cResult.failure)));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>Removes the specified asset pack from the device.</summary>
		/// <param name="assetPackId">The asset pack’s ID.</param>
		/// <returns>A task the yields when the asset pack is removed.</returns>
		public static async Task RemoveAssetPackAsync(string id) {
			RemoveAssetPackContext context = RemoveAssetPackContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_assetpack_remove(id, contextPointer, RemoveAssetPackCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<RemoveAssetPackContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			if (context.GetError() is Error error) {
				throw error;
			}
		}
		
		[MonoPInvokeCallback(typeof(FAssetPackRemove))]
		static void RemoveAssetPackCallback(Error.baw_err cError, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			RemoveAssetPackContext context = Marshal.PtrToStructure<RemoveAssetPackContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetError(new Error(cError));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// The language asset packs that are localized for which the system automatically makes available locally.
		///
		/// The user’s preferred languages inform the choice of resolved language, respecting any language that your app sets manually. The returned value may be null if no localized asset packs are available. If the user recently changed their preferred language, then the returned value could be temporarily out of sync with the set of asset packs that are available locally.
		/// </summary>
		/// <returns>The resolved language.</returns>
		public static Language GetResolvedLanguage() {
			return new Language(baw_assetpackmanager_lang_resolved());
		}
		
		/// <summary>
		/// Sets the language asset packs that are localized for which the system automatically makes available locally.
		///
		/// The user’s preferred languages inform the choice of resolved language, respecting any language that your app sets manually. You can pass null to this method to revert to the user’s system-wide language preference. Setting the language doesn’t immediately download or remove any asset packs; call ReconcilePreferredLanguages() to reconcile the set of downloaded asset packs with the new configuration.
		/// </summary>
		/// <param name="language">The language to set.</param>
		public static void SetResolvedLanguage(Language? language) {
			baw_assetpackmanager_lang_resolved_set(language?.CLanguage ?? new Language.baw_lang());
		}
		
		/// <summary>Returns the languages asset packs that are localized for which are available locally.</summary>
		/// <returns>A task that yields the locally available languages.</returns>
		public static async Task<Language[]> GetLocallyAvailableLanguagesAsync() {
			LocallyAvailableLanguagesContext context = LocallyAvailableLanguagesContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			unsafe {
				baw_assetpackmanager_lang_local(contextPointer, LocallyAvailableLanguagesCallback);
			}
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<LocallyAvailableLanguagesContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			return context.GetLanguages();
		}
		
		[MonoPInvokeCallback(typeof(FLangLocal))]
		unsafe static void LocallyAvailableLanguagesCallback(long count, Language.baw_lang* cLanguages, IntPtr contextPointer) {
			LocallyAvailableLanguagesContext context = Marshal.PtrToStructure<LocallyAvailableLanguagesContext>(contextPointer);
			Language[] languages = new Language[count];
			for (long index = 0; index < count; ++index) {
				languages[index] = new Language(cLanguages[index]);
			}
			context.SetLanguages(languages);
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// Reconciles the set of locally available asset packs with the user’s preferred languages.
		///
		/// This method downloads any missing localized asset packs, waits for those downloads to finish, and removes any unneeded ones. If you’ve overridden the preferred languages, then this method will respect that. It won’t remove any localized asset packs that you’ve downloaded manually.
		/// </summary>
		/// <returns>A task that yields when the reconciliation operation finishes.</returns>
		public static async Task ReconcilePreferredLanguages() {
			ReconcilePreferredLanguagesContext context = ReconcilePreferredLanguagesContext.Create();
			int contextSize = Marshal.SizeOf(context);
			IntPtr contextPointer = Marshal.AllocHGlobal(contextSize);
			Marshal.StructureToPtr(context, contextPointer, false);
			baw_assetpackmanager_lang_reconcile(contextPointer, ReconcilePreferredLanguagesCallback);
			await context.GetSemaphore().WaitAsync();
			context = Marshal.PtrToStructure<ReconcilePreferredLanguagesContext>(contextPointer);
			Marshal.FreeHGlobal(contextPointer);
			if (context.GetError() is Error error) {
				throw error;
			}
		}
		
		static void ReconcilePreferredLanguagesCallback(Error.baw_err cError, Result<object, Error>.baw_res_kind cResultKind, IntPtr contextPointer) {
			ReconcilePreferredLanguagesContext context = Marshal.PtrToStructure<ReconcilePreferredLanguagesContext>(contextPointer);
			switch (cResultKind) {
			case Result<object, Error>.baw_res_kind.baw_res_kind_success:
				break;
			case Result<object, Error>.baw_res_kind.baw_res_kind_failure:
				context.SetError(new Error(cError));
				break;
			}
			Marshal.StructureToPtr(context, contextPointer, true);
			context.GetSemaphore().Release();
		}
		
		/// <summary>
		/// Opens and returns a file handle for an asset file at the specified relative path.
		///
		/// All asset packs share the same namespace, so you can treat the overall collection of downloaded asset packs as if it were a single root directory that contains all of your subdirectories and asset files, regardless of the specific asset pack in which any particular file resides. If there’s a path collision across multiple asset packs, then it’s undefined from which asset pack the file will be read unless you explicitly limit the search to a particular asset pack by passing a non-null ID to the assetPackId parameter.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <param name="assetPackId">The ID of the asset pack in which to search for the file. If you pass null, then all downloaded asset packs are searched.</param>
		/// <returns>A handle for the opened file.</returns>
		public static SafeFileHandle OpenFile(string path, string? assetPackId) {
			Error.baw_err cError;
			int fileDescriptor = baw_assetpackmanager_open(path, assetPackId, out cError);
			if (fileDescriptor < 0) {
				throw new Error(cError);
			}
			return new SafeFileHandle((IntPtr) fileDescriptor, true);
		}
		
		/// <summary>
		/// Opens and returns a file handle for a localized asset file at the specified relative path.
		///
		/// All asset packs share the same namespace, so you can treat the overall collection of downloaded asset packs as if it were a single root directory that contains all of your subdirectories and asset files, regardless of the specific asset pack in which any particular file resides. This method searches in only the downloaded asset packs that are localized in the specified language. If there’s a file-path collision across multiple such asset packs, then it’s undefined from which asset pack the file will be read.
		///
		/// This method is most useful if you intentionally induce a file-path collision across multiple differently localized asset packs. For example, you may include an English-localized version of Videos/Introduction.m4v in an en asset pack, a Hebrew-localized version of Videos/Introduction.m4v in a he asset pack, and an American Spanish–localized version of Videos/Introduction.m4v in an es-US asset pack. If you offer split-language functionality to users, then you may want to download two or more of those asset packs on the same device. In that scenario, the specific choice of file that OpenFile(string, string?) opens would be undefined unless you determine the appropriate asset pack’s ID and pass it to that method’s assetPackId parameter. With this method, merely passing a Language object to the language parameter is sufficient to resolve the ambiguity without requiring that you determine the asset pack’s ID. OpenFile(string, string?) is more suitable in most other situations.
		///
		/// Language matching considers implicit script and region tags per Unicode’s Common Locale Data Repository. For example, en is equivalent to en-US and en-Latn-US but not en-CA.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <param name="language">The language in asset packs that are localized for which to search.</param>
		/// <returns>A handle for the opened file.</returns>
		public static SafeFileHandle OpenLocalizedFile(string path, Language language) {
			Error.baw_err cError;
			int fileDescriptor = baw_assetpackmanager_open_lang(path, language.CLanguage, out cError);
			if (fileDescriptor < 0) {
				throw new Error(cError);
			}
			return new SafeFileHandle((IntPtr) fileDescriptor, true);
		}
		
		/// <summary>
		/// Returns a URI for the specified relative path.
		///
		/// Don’t persist the returned URI beyond the lifetime of the current process.
		///
		/// This method will return a well formed URI even if no item exists at the specified relative path in any asset pack, in which case any attempts to get its contents—whether it’s a file or a directory—will fail.
		///
		/// All asset packs share the same namespace, so you can treat the overall collection of downloaded asset packs as if it were a single root directory that contains all of your subdirectories and asset files, regardless of the specific asset pack in which any particular file resides. Unlike OpenFile(string, string?), this method supports retrieving entire directories—including packages—in which case it merges the corresponding slices of the shared logical directory from all downloaded asset packs that contain such slices. If there’s a path collision across multiple asset packs, then it’s undefined from which asset pack an individual file will be resolved.
		///
		/// This method is less efficient than is OpenFile(string, string?); use that method instead if you can do so. In particular, this method shouldn’t be used to get the URI to the root of the shared asset-pack namespace. Don’t use this method to block the main thread.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <returns>The URI to the item.</returns>
		public static Uri GetUriForFile(string path) {
			Error.baw_err cError;
			IntPtr cUrl = baw_assetpackmanager_url(path, out cError);
			if (cUrl == null) {
				throw new Error(cError);
			}
			string urlString = Marshal.PtrToStringUTF8(cUrl);
			baw_assetpackmanager_url_deinit(cUrl);
			return new Uri(urlString);
		}
		
		/// <summary>
		/// Returns a URI for the specified relative path.
		///
		/// Don’t persist the returned URI beyond the lifetime of the current process.
		///
		/// This method will return a well formed URI even if no item exists at the specified relative path in any relevant asset pack, in which case any attempts to get its contents—whether it’s a file or a directory—will fail.
		///
		/// All asset packs share the same namespace, so you can treat the overall collection of downloaded asset packs as if it were a single root directory that contains all of your subdirectories and asset files, regardless of the specific asset pack in which any particular file resides. Unlike OpenLocalizedFile(string, Language), this method supports retrieving entire directories—including packages—in which case it merges the corresponding slices of the shared logical directory from all downloaded asset packs that are localized in the specified language and that contain such slices. If there’s a path collision across multiple such asset packs, then it’s undefined from which asset pack an individual file will be resolved.
		///
		/// This method is less efficient than is OpenFile(string, Language); use that method instead if you can do so. In particular, this method shouldn’t be used to get the URI to the root of the shared asset-pack namespace. Don’t use this method to block the main thread.
		///
		/// This method is most useful if you intentionally induce a file-path collision across multiple differently localized asset packs. For example, you may include an English-localized version of Videos/Introduction.m4v in an en asset pack, a Hebrew-localized version of Videos/Introduction.m4v in a he asset pack, and an American Spanish–localized version of Videos/Introduction.m4v in an es-US asset pack. If you offer split-language functionality to users, then you may want to download two or more of those asset packs on the same device. In that scenario, the specific choice of item the URI to which GetUriForFile(string) returns would be undefined. With this method, merely passing a Language object to the language parameter is sufficient to resolve the ambiguity. GetUriForFile(string) is more suitable in most other situations.
		///
		/// Language matching considers implicit script and region tags per Unicode’s Common Locale Data Repository. For example, en is equivalent to en-US and en-Latn-US but not en-CA.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <param name="language">The language in asset packs that are localized for which to search.</param>
		/// <returns>The URIF to the item.</returns>
		public static Uri GetUriForLocalizedFile(string path, Language language) {
			Error.baw_err cError;
			IntPtr cUrl = baw_assetpackmanager_url_lang(path, language.CLanguage, out cError);
			if (cUrl == null) {
				throw new Error(cError);
			}
			string urlString = Marshal.PtrToStringUTF8(cUrl);
			baw_assetpackmanager_url_deinit(cUrl);
			return new Uri(urlString);
		}
		
	}
	
}
