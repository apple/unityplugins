using System.Collections.Generic;
using System.Linq;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Apple.BackgroundAssets {
	
	[CreateAssetMenu(menuName = "Apple/Background Assets/Self Hosting Configuration")]
	public class SelfHostingConfiguration: HostingConfiguration {
		
		[Tooltip("The URL of your app’s manifest of asset packs that are available to download from your server.")]
		public string ManifestUrl;
		
		[Tooltip("The domains, including wildcards, from which your app can download asset packs.")]
		public List<string> DownloadDomainAllowList;
		
		[Tooltip("The combined, maximum download size in bytes of asset packs with prefetch download policies that your app can download.")]
		public ulong DownloadAllowance;
		
		[Tooltip("The combined, maximum download size in bytes of asset packs with essential download policies that your app can download.")]
		public ulong EssentialDownloadAllowance;
		
		[Tooltip("The combined, maximum installation size in bytes of asset packs with prefetch download policies that your app can download.")]
		public ulong MaxInstallSize;
		
		[Tooltip("The combined, maximum installation size in bytes of asset packs with essential download policies that your app can download.")]
		public ulong EssentialMaxInstallSize;
		
		internal override (string Old, string New)[] InfoPlistReplacements => this.InfoPlistSentinelReplacements.Select(pair => ($"<integer>{pair.Sentinel}</integer>", $"<integer>{pair.Real}</integer>")).ToArray();
		
		internal List<(int Sentinel, ulong Real)> InfoPlistSentinelReplacements = new List<(int, ulong)>();
		
		const string ManifestUrlKey = "BAManifestURL";
		
		const string InitialDownloadRestrictionsKey = "BAInitialDownloadRestrictions";
		
		const string DownloadDomainAllowListKey = "BADownloadDomainAllowList";
		
		const string DownloadAllowanceKey = "BADownloadAllowance";
		
		const string EssentialDownloadAllowanceKey = "BAEssentialDownloadAllowance";
		
		const string MaxInstallSizeKey = "BAMaxInstallSize";
		
		const string EssentialMaxInstallSizeKey = "BAEssentialMaxInstallSize";
		
		public override void OnProcessInfoPlist(PlistDocument infoPlist) {
			base.OnProcessInfoPlist(infoPlist);
			infoPlist.root.SetBoolean(UsesAppleHostingKey, false);
			infoPlist.root.SetString(ManifestUrlKey, this.ManifestUrl);
			PlistElementDict initialDownloadRestrictions = infoPlist.root.CreateDict(InitialDownloadRestrictionsKey);
			PlistElementArray downloadDomainAllowList = initialDownloadRestrictions.CreateArray(DownloadDomainAllowListKey);
			foreach (string downloadDomain in this.DownloadDomainAllowList) {
				bool downloadDomainIsAlreadyInAllowList = false;
				foreach (PlistElement downloadDomainElement in downloadDomainAllowList.values) {
					if (downloadDomainElement is PlistElementString downloadDomainStringElement && downloadDomainStringElement.value == downloadDomain) {
						downloadDomainIsAlreadyInAllowList = true;
						break;
					}
				}
				if (!downloadDomainIsAlreadyInAllowList) {
					downloadDomainAllowList.AddString(downloadDomain);
				}
			}
			int downloadAllowanceSentinel, essentialDownloadAllowanceSentinel, maxInstallSizeSentinel, essentialMaxInstallSizeSentinel;
			do {
				downloadAllowanceSentinel = Random.Range(1_000_000_000, int.MaxValue);
				essentialDownloadAllowanceSentinel = Random.Range(1_000_000_000, int.MaxValue);
				maxInstallSizeSentinel = Random.Range(1_000_000_000, int.MaxValue);
				essentialMaxInstallSizeSentinel = Random.Range(1_000_000_000, int.MaxValue);
			} while ((new HashSet<int>(new int[] { downloadAllowanceSentinel, essentialDownloadAllowanceSentinel, maxInstallSizeSentinel, essentialMaxInstallSizeSentinel })).Count < 4);
			initialDownloadRestrictions.SetInteger(DownloadAllowanceKey, downloadAllowanceSentinel);
			initialDownloadRestrictions.SetInteger(EssentialDownloadAllowanceKey, essentialDownloadAllowanceSentinel);
			infoPlist.root.SetInteger(MaxInstallSizeKey, maxInstallSizeSentinel);
			infoPlist.root.SetInteger(EssentialMaxInstallSizeKey, essentialMaxInstallSizeSentinel);
			this.InfoPlistSentinelReplacements.Add((downloadAllowanceSentinel, this.DownloadAllowance));
			this.InfoPlistSentinelReplacements.Add((essentialDownloadAllowanceSentinel, this.EssentialDownloadAllowance));
			this.InfoPlistSentinelReplacements.Add((maxInstallSizeSentinel, this.MaxInstallSize));
			this.InfoPlistSentinelReplacements.Add((essentialMaxInstallSizeSentinel, this.EssentialMaxInstallSize));
		}
		
	}
	
}
