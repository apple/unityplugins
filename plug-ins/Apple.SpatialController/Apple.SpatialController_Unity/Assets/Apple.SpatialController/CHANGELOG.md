1.0.6

- Fixed the sample path in `package.json` so it points at the demo's actual location
- Replaced the unresolvable `com.unity.textmeshpro` dependency with `com.unity.ugui`, which is
  what provides TextMeshPro on this plug-in's Unity 6 floor
- Raised the minimum Editor to 6000.5.2f1 and `com.unity.xr.visionos` to 3.1.5, matching the
  versions the plug-in is now built and tested against

1.0.5

- Spatial Stylus controllers now supported

1.0.4

- Controllers with Motion Sensors now supported
- Cleaned up C# and Native code to make it easier to read

1.0.3

- Added support for DualShock touchpad

1.0.2

- fixed logic with bounded check
- documentation in README.md is more descriptive and gives more help

1.0.1

- haptics now provides a stop haptics engine method. This function shuts down an existing engine. 
- cleaning up of folders


1.0.0

- official first release
- package with asmdef files updated and compiling successfully
