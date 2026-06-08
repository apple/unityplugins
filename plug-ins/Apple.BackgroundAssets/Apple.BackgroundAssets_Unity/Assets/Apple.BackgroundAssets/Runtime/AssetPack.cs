using System;
using System.Runtime.InteropServices;

namespace Apple.BackgroundAssets {
	
	/// <summary>An archive of assets that the system downloads together.</summary>
	public class AssetPack {
		
		/// <summary>The status of an asset pack.</summary>
		public class Status {
			
			[StructLayout(LayoutKind.Explicit)]
			internal struct baw_assetpack_status_res {
				
				[FieldOffset(0)]
				internal byte success;
				
				[FieldOffset(0)]
				internal Error.baw_err failure;
				
			}
			
			byte RawValue;
			
			internal Status() {
				this.RawValue = 0;
			}
			
			internal Status(byte rawValue) {
				this.RawValue = rawValue;
			}
			
			/// <summary>Returns whether this status object indicates that the asset pack is available to download.</summary>
			/// <returns>Whether the asset pack is available to download.</returns>
			public bool IsDownloadAvailable() {
				return (this.RawValue & 0b0000001) > 0;
			}
			
			/// <summary>Returns whether this status object indicates that an update to the asset pack is available to download.</summary>
			/// <returns>Whether an update is available to download.</returns>
			public bool IsUpdateAvailable() {
				return (this.RawValue & 0b0000010) > 0;
			}
			
			/// <summary>Returns whether this status object indicates that the downloaded asset pack is up to date.</summary>
			/// <returns>Whether the asset pack is up to date.</returns>
			public bool IsUpToDate() {
				return (this.RawValue & 0b0000100) > 0;
			}
			
			/// <summary>
			/// Returns whether this status object indicates that the downloaded asset pack is out of date.
			///
			/// A return value of true doesn’t necessarily imply that an update to the asset pack can be downloaded over the current network connection. Call IsUpdateAvailable() to determine whether an update can currently be downloaded.
			/// </summary>
			/// <returns>Whether the asset pack is out of date.</returns>
			public bool IsOutOfDate() {
				return (this.RawValue & 0b0001000) > 0;
			}
			
			/// <summary>
			/// Returns whether this status object indicates that the asset pack is no longer available to download.
			///
			/// Obsolete asset packs can’t be updated, and they also can’t be redownloaded once removed.
			/// </summary>
			/// <returns>Whether the asset pack is obsolete.</returns>
			public bool IsObsolete() {
				return (this.RawValue & 0b0010000) > 0;
			}
			
			/// <summary>Returns whether this status object indicates that the system is currently downloading the asset pack.</summary>
			/// <returns>Whether the asset pack is being downloaded.</returns>
			public bool IsDownloading() {
				return (this.RawValue & 0b0100000) > 0;
			}
			
			/// <summary>Returns whether this status object indicates that the system finished downloading the asset pack.</summary>
			/// <returns>Whether the asset pack is downloaded.</returns>
			public bool IsDownloaded() {
				return (this.RawValue & 0b1000000) > 0;
			}
			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct baw_assetpack {
			
			IntPtr impl;
			
		}
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpack_deinit(baw_assetpack assetpack);
		
		[DllImport(InteropUtility.DllName)]
		static extern string baw_assetpack_id(baw_assetpack assetpack);
		
		[DllImport(InteropUtility.DllName)]
		static extern ulong baw_assetpack_downloadsize(baw_assetpack assetpack);
		
		[DllImport(InteropUtility.DllName)]
		static extern ushort baw_assetpack_version(baw_assetpack assetpack);
		
		[DllImport(InteropUtility.DllName)]
		static extern Language.baw_lang baw_assetpack_lang(baw_assetpack assetpack);
		
		internal baw_assetpack CAssetPack;
		
		internal AssetPack(baw_assetpack cAssetPack) {
			this.CAssetPack = cAssetPack;
		}
		
		~AssetPack() {
			baw_assetpack_deinit(this.CAssetPack);
		}
		
		/// <summary>Returns a unique ID for this asset pack.</summary>
		/// <returns>The ID.</returns>
		public string GetId() {
			return baw_assetpack_id(this.CAssetPack);
		}
		
		/// <summary>
		/// Returns the size of the download file containing this asset pack in bytes.
		///
		/// This is different than the installation size, which could be larger.
		/// </summary>
		/// <returns>The download size.</returns>
		public long GetDownloadSize() {
			return (long) baw_assetpack_downloadsize(this.CAssetPack);
		}
		
		/// <summary>Returns this asset pack’s version number.</summary>
		/// <returns>The version number.</returns>
		public ushort GetVersion() {
			return baw_assetpack_version(this.CAssetPack);
		}
		
		/// <summary>Returns the language for which this asset pack is localized.</summary>
		/// <returns>The language.</returns>
		public Language GetLanguage() {
			return new Language(baw_assetpack_lang(this.CAssetPack));
		}
		
	}
	
}
