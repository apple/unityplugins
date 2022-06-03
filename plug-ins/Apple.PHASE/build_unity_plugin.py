#! /usr/bin/env python3
import subprocess, os, argparse, pathlib

# Script to build PHASE Unity plugins and bundle package.
# Options:
#   -build --build-plugin
#   -pack --npm-pack
# Requirements:
#     This script requires npm to be installed in the user's shell.
#     This can be done via Homebrew:
#     $ /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
#     Then we install npm:
#     $ brew install npm
#     To avoid possible permissions errors:
#     $ sudo chown -R $(whoami) /usr/local/share/zsh /usr/local/share/zsh/site-functions
# Example usage:
#     # NOTE: This script should be run with the PHASE Unity open so that Unity will create required .meta files
#
#     # Build a release version
#     $ python3 build_unity_plugin.py -build -pack

# path to where this script lives
script_path = pathlib.Path().absolute()

parser = argparse.ArgumentParser(description="Build PHASE Unity native plugins and create Unity package.")
parser.add_argument("-build", "--build-plugin", dest="build_plugin", action="store_true", help="Build the Unity PHASE native plugins (macOS and iOS).")
parser.add_argument("-pack", "--npm-pack", dest="npm_pack", action="store_true", help="Pack the plugin and assets into a unitypackage using npm.")
flags = parser.parse_args()

# Build native pugin via Xcode
wd = os.getcwd()
if flags.build_plugin:
    os.chdir(script_path.joinpath("UnityPlugins/native/"))
    for x in range(2):
        args = ["xcodebuild"]
        args.append("-scheme")
        if x == 1:
            print("Building AudioPluginPHASE...")
            args.append("AudioPluginPHASE")
        else:
            print("Building AudioPluginPHASE-iOS...")
            args.append("AudioPluginPHASE-iOS")
        args.append("build")
        args.append("-quiet")
        subprocess.run(args)
    os.chdir(wd)
    print("Finished building native plugins.")

# Move to Unity project path
os.chdir(script_path.joinpath("Apple.PHASE_Unity/"))

# Package the unityplugin using npm
if flags.npm_pack:
    print("Packing plugin using npm...")
    subprocess.run(["mv", "Assets/Demos", "Assets/Demos~"])
    subprocess.run(["mv", "Assets/Demos.meta", "../Demos.meta"])
    subprocess.run(["npm", "pack", "./Assets/"])
    subprocess.run(["mv", "Assets/Demos~", "Assets/Demos"])
    subprocess.run(["mv", "../Demos.meta", "Assets/Demos.meta"])
    print("Finished generating PHASE unity plugin package")

os.chdir(wd)

