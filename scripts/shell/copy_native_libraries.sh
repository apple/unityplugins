#!/bin/sh

#  copy_native_libraries.sh
#  Apple Unity Plug-ins
#
#  Copyright © 2024 Apple, Inc. All rights reserved.
#
#  * This script assumes that custom build settings have been configured for the project
#      - UNITY_PROJECT_FOLDER_NAME: This represents the name of the plug-in's Unity project folder
#      - NATIVE_LIBRARY_ROOT_FOLDER_NAME: This represents the name of the folder under the Unity plug-in project's /Assets folder where libraries will be copied
#
#      Note: Please see Core.xcconfig in the Apple.Core project for reference.
#
#  * This script relies upon standard Xcode build settings to be defined and is called from a build post-action for each target. (See each scheme, Build > post-action)
#
#  * Libraries will be copied to:
#      [Plug-in Repository Root]/plug-ins/PluginName/UNITY_PROJECT_FOLDER_NAME/Assets/NATIVE_LIBRARY_ROOT_FOLDER_NAME/NativeLibraries~/$CONFIGURATION/$PLATFORM/LibraryName.suffix
#

printf "Copying built libraries to Unity plug-in folder hierarchy for $TARGET_NAME"

# Determine target platform based upon current SDK
PLATFORM="Unknown"
if [[ $SDKROOT = *'iphoneos'* ]]; then
    PLATFORM="iOS"
elif [[ $SDKROOT = *'macosx'* ]]; then
    PLATFORM="macOS"
elif [[ $SDKROOT = *'appletvos'* ]]; then
    PLATFORM="tvOS"
elif [[ $SDKROOT = *'iphonesimulator'* ]]; then
    PLATFORM="iPhoneSimulator"
elif [[ $SDKROOT = *'appletvsimulator'* ]]; then
    PLATFORM="AppleTVSimulator"
fi

if [[ $PLATFORM == "Unknown" ]]; then
    printf "\nerror: Unable to determine platform from selected SDKROOT!\n"
    exit 1
fi

# Configure & validate path to copy destination root
NATIVE_LIBRARY_DST_ROOT="$PROJECT_DIR/../$UNITY_PROJECT_FOLDER_NAME/Assets/$NATIVE_LIBRARY_ROOT_FOLDER_NAME"

# Make sure that the native library destination folder exists
if [ ! -d $NATIVE_LIBRARY_DST_ROOT ]; then
    printf "\nerror: Native library destination root does not exist at supplied path: $NATIVE_LIBRARY_DST_ROOT\n"
    exit 1
fi

# Define common folder that libraries will be copied to under `NATIVE_LIBRARY_DST_ROOT`
# Trailing tilde is applied so that the resulting folder will be ignored by Unity's build system.
NATIVE_LIBRARY_FOLDER_NAME="NativeLibraries~"

# Set up source and destination paths
src="$BUILT_PRODUCTS_DIR/$WRAPPER_NAME"
dst="$NATIVE_LIBRARY_DST_ROOT/$NATIVE_LIBRARY_FOLDER_NAME/$CONFIGURATION/$PLATFORM/$WRAPPER_NAME"
dsym_src="$src.dSYM"
dsym_dst="$dst.dSYM"

# Make sure there's something at the source path
if [ -e $src ]; then
    # If there's already something at the destination path, remove it
    if [ -e $dst ]; then
        printf "\nRemoving existing library at: $dst\n"
        rm -rf $dst
    fi

    printf "\nCopying Library:\n  Source: $src\n  Destination: $dst\n"
    ditto $src $dst
  
    # If we're in a release build and there's a dSYM, copy it as well.
    if [[ $CONFIGURATION == "Release" && -e $dsym_src ]]; then
        # If there's already a .dSYM, remove it
        if [ -e $dsym_dst ]; then
            printf "\nRemoving existing .dSYM at: $dsym_dst\n"
            rm -rf $dsym_dst
        fi
    
        printf "\nCopying dSYM:\n  Source: $dsym_src\n  Destination: $dsym_dst\n"
        ditto $dsym_src $dsym_dst
    fi
fi
