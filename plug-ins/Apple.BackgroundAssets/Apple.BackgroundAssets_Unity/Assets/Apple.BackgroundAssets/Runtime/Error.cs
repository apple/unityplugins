using System;
using System.Runtime.InteropServices;

namespace Apple.BackgroundAssets {
	
	/// <summary>An error that the Background Assets framework throws.</summary>
	public class Error : Exception {
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct baw_err {
			
			internal IntPtr description;

			// C '_Bool', held as a byte rather than a bool so that baw_err stays blittable. A bool
			// would make every struct embedding this one non-blittable, and IL2CPP would then marshal
			// those field by field — which silently corrupts the unions that overlay baw_err with a
			// success value. [MarshalAs] does not help: it sets the width of the conversion, it does
			// not remove it. See Tests/TestInteropBlittability.cs.
			byte _static;
			
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
