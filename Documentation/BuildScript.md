| [Home](../README.md) | [Feedback](Feedback.md) | [Quick Start Guide](Quickstart.md) | [Build Script](BuildScript.md) |
| :---: | :---: | :---: | :---: |


# Apple Unity Plug-In Build Script Usage

## Requirements
* Python3
* npm
* Xcode
* Unity 2020.3.33f1

:exclamation: Once the plug-ins have been built, they can be used with any newer Unity version. However, it is recommended to first build plug-ins with the suggested Unity version.

## Basic Usage

As discussed in the [QuickStart Guide](Quickstart.md), the fastest way to begin working with the Apple Unity plug-ins is to run:

```bash
python3 build.py
```

This command will build each plug-in's native libraries and `npm pack` the entire plug-in into a `.tgz` file for easily incorporating into a Unity project via the Unity Package Manager.

## Advanced Usage

Running `python3 build.py --help` will show the list of flags available for customizing the build process. What follows is a description of what each flag does and some example use cases.
* [Plug-in Selection](#Plug-in-Selection)
* [Build Actions](#Build-Actions)
* [Unity Installation Path](#Unity-Installation-Path)
* [Debug Builds](#Debug-Builds)
* [Output Path](#Output-Path)
* [Clean Actions](#Clean-Actions)
* [Force Clean](#Force-Clean)
* [Test Builds](#Test-Builds)
* [Test Output Path](#Test-Output-Path)


### Plug-in Selection
- **Flag:** `--plugin-list`
- **Short version:** `-p`
- **Possible values:** `all`, `Core`, `Accessibility`, `CoreHaptics`, `GameController`, `GameKit`, `PHASE`
- **Default value:** `all`
- **Description:** Selects a subset of plug-ins to perform the [build](#Build-Actions), [clean](#Clean-Actions), or [test](#Test-Builds) action or actions selected. For example, you may want to perform the default actions on only a subset of plug-ins, such as Apple.Core, Apple.GameKit, and Apple.GameController. This can be done by running:

```bash
python3 build.py -p Core GameKit GameController
```


### Build Actions
- **Flag:** `--build-action`
- **Short version:** `-b`
- **Possible values:** `all`, `none`, `build`, `pack`
- **Default value:** `all`
- **Description:** Sets the build action(s) to perform for the selected plug-ins. For example, you may choose only to `build` in order to explore the sample projects included with each plug-in, or to reference the plug-ins for local development rather than referencing a read-only tarball. This can be done by running:

```bash
python3 build.py -b build
```


### Unity Installation Path
- **Flag:** `--unity-installation-root`
- **Short version:** `-u`
- **Possible values:** Any system directory
- **Default value:** `/Applications/Unity`
- **Description:** The build actions above require a valid Unity installation in order to properly resolve the native library references within the Unity package. This flag is used to provide a path to use for searching for Unity installations. Most users will likely have their Unity installations installed in the `/Applications/Unity/` path. However, if your Unity settings are custom, you may have elected to use an alternate location, such as `~/Desktop/UnityInstalls/`. In that case, tell the build script where to find your Unity installations by running:

```bash
python3 build.py -u ~/Desktop/UnityInstalls
```

:warning: The path provided is used as the basis for a full-depth recursive search. Providing a system path such as `/` or `/Users/` could result in very long build times.


### Debug Builds
- **Flag:** `--debug`
- **Short version:** `-d`
- **Default value:** Off
- **Description:** If you are making changes to the native code as part of an open source contribution, or if for any other reason you'd like to a debug build with more verbose native library logging, add this flag to your build command. For example:

```bash
python3 build.py -d
```


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












