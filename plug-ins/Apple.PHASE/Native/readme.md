## Summary

Steps to build the PHASE native Unity plugin for MacOs and iOS.

## Understanding and Building the Native Plugin

- Open the AudioPluginPHASE XCode Project 

- You'll notice that it builds the PHASE Framework and has two very simple header / source files called PHASEAPI.h/.m

- If you look inside the PHASEAPI.h file you'll see it's just an extern "C" function that returns an int. In it's implementation in the .mm file it actually interacts with the PHASE Framework and tries and creates the PHASEEngine object as an example.

- This project outputs a .bundle for MacOs (since Unity loads MacOS plugins as dynamically linked libraries) and a .a static library for iOS (since Unity iOS can only use statically linked libraries)

- You can build the .bundle and .a static library and it should succeed. These should only take a couple of seconds.

- These libraries can be imported into Unity and be used inside Unity object scripts. 

## Building and Running the Unity test project

### iOS

- Open the TestUnityProject

- Go to File -> Build Settings , select iOS or macOS and click on Switch Platform

- Click on Build. This will prompt a file save dialog as Unity will actually create an Xcode project that can be built in Xcode and ran on the device.

- Some more manual things have to be made here that can be automated but for now:
 - Open the Xcode project 
 - Connect your device and build the application
 - Select the TestUnityProject app on your iOS device to run it

## MacOs

- Open the TestUnityProject

- Go to File -> Build Settings , select "PC, Mac & Linux Standalone" and click on Switch Platform

- Click on Build. This will build the project as a .app bundle with the filename of your choice
