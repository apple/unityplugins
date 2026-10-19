using System;

namespace Apple.BackgroundAssets {
	
	class Result<TSuccess, TFailure> where TFailure : Exception {
		
		#nullable enable
		
		internal enum baw_res_kind {
			baw_res_kind_success,
			baw_res_kind_failure
		}
		
		readonly TSuccess? Success;
		
		readonly TFailure? Failure;
		
		public Result() {
			this.Success = default(TSuccess);
			this.Failure = default(TFailure);
		}
		
		internal Result(TSuccess success) {
			this.Success = success;
			this.Failure = default(TFailure);
		}
		
		internal Result(TFailure failure) {
			this.Success = default(TSuccess);
			this.Failure = failure;
		}
		
		internal TSuccess Get() {
			if (this.Success == null) {
				throw this.Failure ?? new Exception();
			} else {
				return this.Success;
			}
		}
		
	}
	
}
