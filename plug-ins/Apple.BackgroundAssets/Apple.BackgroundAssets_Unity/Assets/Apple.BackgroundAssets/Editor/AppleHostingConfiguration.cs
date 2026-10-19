using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Apple.BackgroundAssets {
	
	[CreateAssetMenu(menuName = "Apple/Background Assets/Apple Hosting Configuration")]
	public class AppleHostingConfiguration: HostingConfiguration {
		
		public override void OnProcessInfoPlist(PlistDocument infoPlist) {
			base.OnProcessInfoPlist(infoPlist);
			infoPlist.root.SetBoolean(UsesAppleHostingKey, true);
		}
		
	}
	
}
