using System;
using System.Runtime.InteropServices;

namespace Apple.BackgroundAssets {
	
	/// <summary>An error that the Background Assets framework throws.</summary>
	public class Error : Exception {
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct baw_err {
			
			internal IntPtr description;
			
			bool _static;
			
		}
		
		[DllImport(InteropUtility.DllName)]
		static extern void baw_err_deinit(baw_err err);
		
		baw_err CError;
		
		internal Error(baw_err cError) {
			this.CError = cError;
		}
		
		~Error() {
			baw_err_deinit(this.CError);
		}
		
		public unsafe override string ToString() {
			return Marshal.PtrToStringUTF8(this.CError.description);
		}
		
	}
	
}
