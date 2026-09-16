using System;
using System.Runtime.InteropServices;

namespace Apple.BackgroundAssets {
	
	/// <summary>A language, including its script and region.</summary>
	public class Language {
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct baw_lang {
			
			IntPtr impl;
			
		}
		
		[DllImport(InteropUtility.DllName)]
		static extern baw_lang baw_lang_init(string id);
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_lang_deinit(baw_lang lang);
		
		[DllImport(InteropUtility.DllName)]
		internal static extern bool baw_lang_is_nonnull(baw_lang lang);
		
		[DllImport(InteropUtility.DllName)]
		static extern string baw_lang_id_min(baw_lang lang);
		
		[DllImport(InteropUtility.DllName)]
		static extern string baw_lang_id_max(baw_lang lang);
		
		[DllImport(InteropUtility.DllName)]
		static extern bool baw_lang_equivalent(baw_lang lang_first, baw_lang lang_second);
		
		internal baw_lang CLanguage;
		
		/// <summary>Creates a language from a BCP-47 ID.</summary>
		/// <param name="id">A BCP-47 ID.</param>
		public Language(string id) {
			this.CLanguage = baw_lang_init(id);
		}
		
		internal Language(baw_lang cLanguage) {
			this.CLanguage = cLanguage;
		}
		
		~Language() {
			baw_lang_deinit(this.CLanguage);
		}
		
		/// <summary>Returns this language’s minimal BCP-47 ID, excluding subtags that the system can infer implicitly.</summary>
		/// <returns>The minimal ID.</returns>
		public string GetMinimalId() {
			return baw_lang_id_min(this.CLanguage);
		}
		
		/// <summary>Returns this language’s maximal BCP-47 ID, including all subtags.</summary>
		/// <returns>The maximal ID.</returns>
		public string GetMaximalId() {
			return baw_lang_id_max(this.CLanguage);
		}
		
		/// <summary>Returns whether this language is equivalent to another language, considering implied subtags.</summary>
		/// <param name="other">Another language.</param>
		/// <returns>Whether the languages are equivalent.</returns>
		public bool IsEquivalent(Language other) {
			return baw_lang_equivalent(this.CLanguage, other.CLanguage);
		}
		
	}
	
}
