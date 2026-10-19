using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Apple.BackgroundAssets {
	
	public abstract class HostingConfiguration: ScriptableObject {
		
		internal virtual (string Old, string New)[] InfoPlistReplacements => new (string, string)[0];
		
		const string HasManagedAssetPacksKey = "BAHasManagedAssetPacks";
		
		protected const string UsesAppleHostingKey = "BAUsesAppleHosting";
		
		public virtual void OnProcessInfoPlist(PlistDocument infoPlist) {
			infoPlist.root.SetBoolean(HasManagedAssetPacksKey, true);
		}
		
	}
	
}
