using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Apple.Core
{
    public class AppleSecurityBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Security";
        
        [Tooltip("If true, will add the com.apple.security.app-sandbox entitlement.")]
        public bool AppSandboxEntitlement = true;

        [Header("Network")] 
        [Tooltip("A Boolean value indicating whether your app may listen for incoming network connections.")]
        public bool AllowNetworkServer;
        [Tooltip("A Boolean value indicating whether your app may open outgoing network connections.")]
        public bool AllowNetworkClient;

        [Header("Hardware")] 
        [Tooltip("A Boolean value that indicates whether the app may capture movies and still images using the built-in camera.")]
        public bool AllowCamera;
        [Tooltip("A Boolean value that indicates whether the app may use the microphone.")]
        public bool AllowMicrophone;
        [Tooltip("A Boolean value indicating whether your app may interact with USB devices.")]
        public bool AllowUsb;
        [Tooltip("A Boolean value indicating whether your app may print a document.")]
        public bool AllowPrint;
        [Tooltip("A Boolean value indicating whether your app may interact with Bluetooth devices.")]
        public bool AllowBluetooth;

        [Header("AppData")] 
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to contacts in the user's address book.")]
        public bool AllowAddressBook;
        [Tooltip("A Boolean value that indicates whether the app may access location information from Location Services.")]
        public bool AllowLocation;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to the user's calendar.")]
        public bool AllowCalendars;

        [Header("File Access")] 
        [Tooltip("A Boolean value that indicates whether the app may have read-only access to files the user has selected using an Open or Save dialog.")]
        public bool AllowUserSelectedReadOnly;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to files the user has selected using an Open or Save dialog.")]
        public bool AllowUserSelectedReadWrite;
        [Tooltip("A Boolean value that indicates whether the app may have read-only access to the Downloads folder.")]
        public bool AllowDownloadsReadOnly;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to the Downloads folder.")]
        public bool AllowDownloadsReadWrite;
        [Tooltip("A Boolean value that indicates whether the app may have read-only access to the Pictures folder.")]
        public bool AllowPicturesReadOnly;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to the Pictures folder.")]
        public bool AllowPicturesReadWrite;
        [Tooltip("A Boolean value that indicates whether the app may have read-only access to the Music folder.")] 
        public bool AllowMusicReadOnly;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to the Music folder.")]
        public bool AllowMusicReadWrite;
        [Tooltip("A Boolean value that indicates whether the app may have read-only access to the Movies folder.")]
        public bool AllowMoviesReadOnly;
        [Tooltip("A Boolean value that indicates whether the app may have read-write access to the Movies folder.")]
        public bool AllowMoviesReadWrite;

        public override void OnProcessEntitlements(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument entitlements)
        {
            if (AppSandboxEntitlement)
                entitlements.root.SetBoolean("com.apple.security.app-sandbox", true);

            // Network...
            if(AllowNetworkServer)
                entitlements.root.SetBoolean("com.apple.security.network.server", true);
            
            if(AllowNetworkClient)
                entitlements.root.SetBoolean("com.apple.security.network.client", true);
            
            // Hardware....
            if(AllowCamera)
                entitlements.root.SetBoolean("com.apple.security.device.camera", true);
            
            if(AllowMicrophone)
                entitlements.root.SetBoolean("com.apple.security.device.microphone", true);
            
            if(AllowUsb)
                entitlements.root.SetBoolean("com.apple.security.device.usb", true);
            
            if(AllowPrint)
                entitlements.root.SetBoolean("com.apple.security.print", true);
            
            if(AllowBluetooth)
                entitlements.root.SetBoolean("com.apple.security.device.bluetooth", true);
            
            // AppData...
            if(AllowAddressBook)
                entitlements.root.SetBoolean("com.apple.security.personal-information.addressbook", true);
            
            if(AllowLocation)
                entitlements.root.SetBoolean("com.apple.security.personal-information.location", true);
            
            if(AllowCalendars)
                entitlements.root.SetBoolean("com.apple.security.personal-information.calendars", true);
            
            // File access...
            if(AllowUserSelectedReadOnly)
                entitlements.root.SetBoolean("com.apple.security.files.user-selected.read-only", true);
            
            if(AllowUserSelectedReadWrite)
                entitlements.root.SetBoolean("com.apple.security.files.user-selected.read-write", true);
            
            if(AllowDownloadsReadOnly)
                entitlements.root.SetBoolean("com.apple.security.files.downloads.read-only", true);
            
            if(AllowDownloadsReadWrite)
                entitlements.root.SetBoolean("com.apple.security.files.downloads.read-write", true);
            
            if(AllowPicturesReadOnly)
                entitlements.root.SetBoolean("com.apple.security.assets.pictures.read-only", true);
            
            if(AllowPicturesReadWrite)
                entitlements.root.SetBoolean("com.apple.security.assets.pictures.read-write", true);
            
            if(AllowMusicReadOnly)
                entitlements.root.SetBoolean("com.apple.security.assets.music.read-only", true);

            if(AllowMusicReadWrite)
                entitlements.root.SetBoolean("com.apple.security.assets.music.read-write", true);
            
            if(AllowMoviesReadOnly)
                entitlements.root.SetBoolean("com.apple.security.assets.movies.read-only", true);

            if(AllowMoviesReadWrite)
                entitlements.root.SetBoolean("com.apple.security.assets.movies.read-write", true);
        }
    }
}