# Apple.SpatialAccessory Support

## Description

A Unity plugin for Spatial Accessory Tracking + Game Controller

## Import Unity Package

1. Create a new project (see instructions from build.py) and import locally the unity package from the Unity directory
2. The dependencies will also be installed
3. Open Apple -> Spatial Accessory Tracking editor window from the menu bar and follow the scene setup


## Configure Unity Project for visionOS

1. In Project Settings -> XR Plug-in Management -> Apple visionOS, select any desired App Mode. 
	* *NOTE: In Windowed mode, VolumeCamera Volume Window Mode Bounded is not currently supported.*
2. In Build Profiles -> visionOS, set visionOS Settings -> Run in Xcode to v.26.0 or later Xcode.app
3. In Build Profiles -> visionOS, press Build button to build the Unity-VisionOS xcode project

