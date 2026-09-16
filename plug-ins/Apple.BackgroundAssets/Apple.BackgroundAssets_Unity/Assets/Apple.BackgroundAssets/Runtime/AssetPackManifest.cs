using System.Runtime.InteropServices;

namespace Apple.BackgroundAssets {
	
	/// <summary>A manifest of asset packs that are available to download.</summary>
	public class AssetPackManifest {
		
		[StructLayout(LayoutKind.Sequential)]
		struct baw_assetpackmanifest_compat {
			
			long assetpackc;
			
			unsafe AssetPack.baw_assetpack* assetpackv;
			
		}
		
		[StructLayout(LayoutKind.Explicit)]
		internal struct baw_assetpackmanifest {
			
			[FieldOffset(0)]
			unsafe void* opaque;
			
			[FieldOffset(0)]
			baw_assetpackmanifest_compat compat;
		}
		
		[StructLayout(LayoutKind.Explicit)]
		internal struct baw_assetpackmanifest_res {
			
			[FieldOffset(0)]
			internal baw_assetpackmanifest success;
			
			[FieldOffset(0)]
			internal Error.baw_err failure;
			
		}
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_assetpackmanifest_deinit(baw_assetpackmanifest manifest);
		
		[DllImport(InteropUtility.DllName)]
		static extern AssetPack.baw_assetpack baw_assetpackmanifest_assetpack(baw_assetpackmanifest manifest, string id);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern AssetPack.baw_assetpack* baw_assetpackmanifest_assetpackv(baw_assetpackmanifest manifest, out long assetpackc);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern AssetPack.baw_assetpack* baw_assetpackmanifest_assetpackv_localized(baw_assetpackmanifest manifest, out long assetpackc);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern AssetPack.baw_assetpack* baw_assetpackmanifest_assetpackv_localized_lang(baw_assetpackmanifest manifest, Language.baw_lang lang, out long assetpackc);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern void baw_assetpackmanifest_assetpackv_deinit(long assetpackc, AssetPack.baw_assetpack* assetpackv);
		
		[DllImport(InteropUtility.DllName)]
		static extern Language.baw_lang baw_assetpackmanifest_lang_primary(baw_assetpackmanifest manifest);
		
		[DllImport(InteropUtility.DllName)]
		static extern Language.baw_lang baw_assetpackmanifest_lang_resolved(baw_assetpackmanifest manifest);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern Language.baw_lang* baw_assetpackmanifest_langv(baw_assetpackmanifest manifest, out long langc);
		
		[DllImport(InteropUtility.DllName)]
		unsafe static extern void baw_assetpackmanifest_langv_deinit(long langc, Language.baw_lang* langv);
		
		internal baw_assetpackmanifest CManifest;
		
		internal AssetPackManifest(baw_assetpackmanifest cManifest) {
			this.CManifest = cManifest;
		}
		
		~AssetPackManifest() {
			baw_assetpackmanifest_deinit(this.CManifest);
		}
		
		/// <summary>Returns the asset pack in this manifest with the given ID.</summary>
		/// <param name="id">The asset pack’s ID.</param>
		/// <returns>The asset pack.</returns>
		public AssetPack GetAssetPack(string id) {
			return new AssetPack(baw_assetpackmanifest_assetpack(this.CManifest, id));
		}
		
		/// <summary>Returns the asset packs in this manifest that are available to download.</summary>
		/// <returns>The asset packs</returns>
		public unsafe AssetPack[] GetAllAssetPacks() {
			long count;
			AssetPack.baw_assetpack* cAssetPacks = baw_assetpackmanifest_assetpackv(this.CManifest, out count);
			if (cAssetPacks == null) {
				return new AssetPack[0];
			} else {
				AssetPack[] assetPacks = new AssetPack[count];
				for (long index = 0; index < count; ++index) {
					assetPacks[index] = new AssetPack(cAssetPacks[index]);
				}
				baw_assetpackmanifest_assetpackv_deinit(count, cAssetPacks);
				return assetPacks;
			}
		}
		
		/// <summary>Returns the subset of asset packs in this manifest that are available to download and that best match the user’s preferred languages.</summary>
		/// <returns>The localized asset packs.</returns>
		public unsafe AssetPack[] GetLocalizedAssetPacks() {
			long count;
			AssetPack.baw_assetpack* cAssetPacks = baw_assetpackmanifest_assetpackv_localized(this.CManifest, out count);
			if (cAssetPacks == null) {
				return new AssetPack[0];
			} else {
				AssetPack[] assetPacks = new AssetPack[count];
				for (long index = 0; index < count; ++index) {
					assetPacks[index] = new AssetPack(cAssetPacks[index]);
				}
				baw_assetpackmanifest_assetpackv_deinit(count, cAssetPacks);
				return assetPacks;
			}
		}
		
		/// <summary>
		/// Returns the subset of asset packs in this manifest that are available to download and that best match the specified language.
		///
		/// Depending on which languages are available, the returned asset packs’ respective languages may not exactly match the specified language.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns>The localized asset packs.</returns>
		public unsafe AssetPack[] GetLocalizedAssetPacks(Language language) {
			long count;
			AssetPack.baw_assetpack* cAssetPacks = baw_assetpackmanifest_assetpackv_localized_lang(this.CManifest, language.CLanguage, out count);
			if (cAssetPacks == null) {
				return new AssetPack[0];
			} else {
				AssetPack[] assetPacks = new AssetPack[count];
				for (long index = 0; index < count; ++index) {
					assetPacks[index] = new AssetPack(cAssetPacks[index]);
				}
				baw_assetpackmanifest_assetpackv_deinit(count, cAssetPacks);
				return assetPacks;
			}
		}
		
		/// <summary>
		/// Returns the app’s primary language as configured in App Store Connect.
		///
		/// If no available localized asset packs match the user’s preferred languages, then the system will fall back on the app’s primary language.
		/// </summary>
		/// <returns>The primary language.</returns>
		public Language GetPrimaryLanguage() {
			Language.baw_lang cLanguage = baw_assetpackmanifest_lang_primary(this.CManifest);
			if (Language.baw_lang_is_nonnull(cLanguage)) {
				return new Language(cLanguage);
			} else {
				return null;
			}
		}
		
		/// <summary>
		/// Returns the language asset packs in this manifest that are localized for which the system automatically makes available locally.
		///
		/// The user’s preferred languages inform the choice of resolved language, respecting any language that your app sets manually. This property may be nil if no localized asset packs are available. If the user recently changed their preferred language or if this manifest is outdated, then this property’s value may be out of sync with the set of asset packs that are available locally.
		/// </summary>
		/// <returns>The resolved language.</returns>
		public Language GetResolvedLanguage() {
			Language.baw_lang cLanguage = baw_assetpackmanifest_lang_resolved(this.CManifest);
			if (Language.baw_lang_is_nonnull(cLanguage)) {
				return new Language(cLanguage);
			} else {
				return null;
			}
		}
		
		/// <summary>Returns the languages for which asset packs in this manifest are localized.</summary>
		/// <returns>The available languages.</returns>
		public unsafe Language[] GetAvailableLanguages() {
			long count;
			Language.baw_lang* cLanguages = baw_assetpackmanifest_langv(this.CManifest, out count);
			if (cLanguages == null) {
				return new Language[0];
			} else {
				Language[] languages = new Language[count];
				for (long index = 0; index < count; ++index) {
					languages[index] = new Language(cLanguages[index]);
				}
				baw_assetpackmanifest_langv_deinit(count, cLanguages);
				return languages;
			}
		}
		
	}
	
}
