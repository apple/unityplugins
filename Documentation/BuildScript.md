| [Home](../README.md) | [Feedback](Feedback.md) | [Quick Start Guide](Quickstart.md) | [Build Script](BuildScript.md) |
| :---: | :---: | :---: | :---: |


# Apple Unity Plug-In Build Script Usage

## Overview
These instructions are for the Apple Unity Plug-in Build Script, build.py, version 2.0.0. This updated version of the build script allows for increased flexibility by adding a few key features:
* Easily configure or disable prompt output colors. (See the section 'Prompt Formatting' beginning at line 19 in build.py)
* Enable a codesign workflow to sign compiled libraries, a requirement for recent Unity builds
* Allow for upgrade of the Unity plug-in projects, removing the strict requirement of Unity 2020.3.33f1 to build (Though 2020.3.33f1 *or newer* is necessary)

## Requirements
* Python3 - To run `build.py`
* npm - When packing into .tgz files
* Xcode - For compilation of native libraries
* Unity 2020.3.33f1 or newer when building tests.
  * See: [Test Builds](#test-builds)

## Basic Usage

As discussed in the [QuickStart Guide](Quickstart.md), the fastest way to begin working with the Apple Unity plug-ins is to run:

```bash
python3 build.py
```

This command will build each plug-in's native libraries and `npm pack` the entire plug-in into a `.tgz` file for easily incorporating into a Unity project via the Unity Package Manager.

## Advanced Usage

Running `python3 build.py --help` will show the list of flags available for customizing the build process. What follows is a description of what each flag does and some example use cases.
* [Plug-in Selection](#plug-in-selection)
* [Platform Selection](#platform-selection)
* [Build Actions](#build-actions)
* [Code Signing](#code-signing)
* [Skip Code Sign](#skip-code-sign)
* [Unity Installation Path](#unity-installation-path)
* [Debug Builds](#debug-builds)
* [Output Path](#output-path)
* [Clean Actions](#clean-actions)
* [Force Clean](#force-clean)
* [Test Builds](#test-builds)
* [Test Output Path](#test-builds)

### Plug-in Selection
- **Flag:** `--plugin-list`
- **Short version:** `-p`
- **Possible values:** `all`, `Core`, `Accessibility`, `CoreHaptics`, `GameController`, `GameKit`, `PHASE`
- **Default value:** `all`
- **Description:** Selects a subset of plug-ins to perform the [build](#build-actions), [clean](#clean-actions), or [test](#test-builds) action or actions selected. For example, you may want to perform the default actions on only a subset of plug-ins, such as Apple.Core, Apple.GameKit, and Apple.GameController. This can be done by running:

```bash
python3 build.py -p Core GameKit GameController
```

### Platform Selection
- **Flag:** `--platforms`
- **Short version:** `-m`
- **Possible values:** `all`, `iOS`, `macOS`, `tvOS`, `iPhoneSimulator`, `AppleTVSimulator`, `simulators`, `devices`
- **Default value:** `all`
- **Description:** Selects the desired platforms that the plug-ins will target, which also limits which native libraries will be compiled. Platforms are validated against the build machine's installed set of Apple platform SDKs. Choosing the option `simulators` will build all simulator platforms while the option `devices` builds all device platforms. 

:new: Build option `all` now only builds for platforms supported by the build machine. If no tvOS SDK is installed, `all` will no longer select tvOS support.
:new: Explicitly choosing an unsupported SDK will prompt the user to install the missing SDK.

Example: Build all plug-ins but only target the iPhone simulator, AppleTV simulator, and macOS:

```bash
python3 build.py -m simulators macOS
```
:warning: **Note:** Not building with `macOS` support will result in losing play mode functionality in the Editor. It's recommended to always include `macOS` support.

### Build Actions
- **Flag:** `--build-action`
- **Short version:** `-b`
- **Possible values:** `all`, `none`, `build`, `pack`
- **Default value:** `all`
- **Description:** Sets the build action(s) to perform for the selected plug-ins. For example, you may choose only to `build` in order to explore the sample projects included with each plug-in, or to reference the plug-ins for local development rather than referencing a read-only tarball. This can be done by running:

```bash
python3 build.py -b build
```

### Build Configs
- **Flag:** `--build-config`
- **Short version:** `-bc`
- **Possible values:** `all`, `Release`, `Debug`
- **Default value:** `all`
- **Description:** This option allows for the build of both `Release` and `Debug` variants of the underlying native libraries for all plug-ins. When available, the plug-in build processing pipeline will use `Debug` libraries for **Development** builds in the Editor and `Release` otherwise. If no variants are available, the plug-in build processing pipeline will use whichever configuration is available. For example, to build release plug-ins for all selected platforms run:

```bash
python3 build.py -bc Release
```
:no_entry_sign: **Important:** The debug build flag `--debug` (`-d`) has been obsoleted and is now replaced by `--build-config` (`-bc`)

### Code Signing
- **Flag:** `--codesign-identity`
- **Short version:** `-c`
- **Possible values:** Any (case sensitive) substring of the code signing certificate's common name attribute, as long as the substring is unique throughout your keychains.
- **Default value:** None
- **Description:** Determines the codesign identity to be used when compiling native libraries. Note that this step is not strictly necessary, but recent versions of Unity require native plug-in libraries to be signed to load. For more information on codesigning, please see the Apple Developer Documentation article [Code Signin Guide: Signing Code Manually](https://developer.apple.com/library/archive/documentation/Security/Conceptual/CodeSigningGuide/Procedures/Procedures.html#//apple_ref/doc/uid/TP40005929-CH4-SW4).
- **Note:** If no codesign identity is provided, user will be prompted. To decline codesign and skip user prompt, please see [Skip Code Sign](#skip-code-sign)

### Skip Code Sign
- **Flag:** `--skip-codesign`
- **Short version:** `-sc`
- **Default value:** Off
- **Description:** Setting this flag skips both code signing and user prompt

### Unity Installation Path
- **Flag:** `--unity-installation-root`
- **Short version:** `-u`
- **Possible values:** Any system directory
- **Default value:** `/Applications/Unity`
- **Description:** The build actions above require a valid Unity installation in order to select the correct version of Unity when building tests. This flag is used to provide a path to use for searching for Unity installations. Most users will likely have their Unity installations installed in the `/Applications/Unity/` path. However, if your Unity settings are custom, you may have elected to use an alternate location, such as `~/Desktop/UnityInstalls/`. In that case, tell the build script where to find your Unity installations by running:

```bash
python3 build.py -u ~/Desktop/UnityInstalls
```
:warning: The path provided is used as the basis for a full-depth recursive search. Providing a system path such as `/` or `/Users/` could result in very long build times.
:new::exclamation: This argument now only applies when building tests. Unity is no longer needed for verification during non-test builds.

### Output Path
- **Flag:** `--output-path`
- **Short version:** `-o`
- **Possible values:** Any relative system directory
- **Default value:** `Build/` (a directory created at the repo root)
- **Description:** Used to select a directory for the packaged plug-in `.tgz` tarballs. For example, to build and pack all of the plug-ins and put those `.tgz` files on your Desktop, run:

```bash
python3 build.py -o ~/Desktop/
```

### Clean Actions
- **Flag:** `--clean-action`
- **Short version:** `-k`
- **Possible values:** `all`, `none`, `native`, `packages`, `tests`
- **Default value:** `none`
- **Description:** Sets the clean action(s) to perform for the selected plug-ins. For example, you may want to remove any `.tgz` packages before building new ones and also rebuild the native libraries from scratch. This is accomplished by running:

```bash
python3 build.py -k packages native
```

:exclamation: Note that this attempts to clean files only at the specified output path, not at paths used by previous uses of the script.

### Force Clean
- **Flag:** `--force`
- **Short version:** `-f`
- **Default value:** Off
- **Description:** If performing clean actions that would normally prompt the user for permission, override any prompts. For example, you may choose to run:

```bash
python3 build.py -k all -f
```

### Test Builds
- **Flag:** `--test`
- **Short version:** `-t`
- **Default value:** Off
- **Description:** If you would like to run the C# test suites included with each selected plug-in, include this flag in your build command. This may be especially useful if you choose to contribute to the open-source project. Any changes submitted must pass all existing tests, and hopefully include some new tests to verify the changes! Each plug-in's tests will build into a separate Xcode project, placed in the `TestBuilds/`. To run the test suite(s), open this resulting Xcode project, then Build & Run the application on the desired device(s). Test results can be found in the Xcode logger. For example, if you're making changes to the CoreHaptics plug-in and want to verify its tests all pass, run:

```bash
python3 build.py -p CoreHaptics -t -b build
```
:exclamation: Note that this command elects only to `build`, and not `pack` the plug-in. If you are iterating only on the Unity code and not making any changes to the native code, you might opt for `-b none` here to reduce test building time.
:warning: Adding this option will verify your Unity installation against the verion(s) of the plug-ins for which you are building tests. The script will prompt for attempted upgrade, but best practice is to match your Unity installation to the project version(s).

### Test Output Path
- **Flag:** `--test-output-path`
- **Short version:** `-to`
- **Possible values:** Any relative system directory
- **Default value:** `TestBuilds/` (a directory created at the repo root)
- **Description:** If you have elected to build the test suites with `--test` above, this argument can be used to select a specific location to output the resulting Xcode projects. For example, if you want to run the CoreHaptics test suite and have the 

```bash
python3 build.py -t -to
```

[^ Back to Top](#Apple-Unity-Plug-In-Build-Script-Usage)












