#! /usr/bin/env python3
# Requirements: python3
# Contains a collection of utility methods.

import pathlib
import sys, subprocess

# --------------------------------
# Helper methods for script output

# Outputs a message string in red and optionally terminates the script
# Note: Cleanup, such as resetting the working directory, should be performed prior to calling with bExit = True.
def ErrorMessage(sMessage, bExit = False):
    print(f"\033[91m[ERROR]: {sMessage}\033[00m")
    if (bExit):
        sys.exit()

# Outputs a warning message in yellow
def WarningMessage(sMessage):
    print(f"\033[93m[WARNING]: {sMessage}\033[00m")

# Outputs a status message in cyan
def StatusMessage(sMessage):
    print(f"\033[96m{sMessage}\033[00m")

# Removes data within the folder at 'folder_path'
# Keeps folder in place but removes all contents, except .gitignore and .npmignore files, when 'contents_only' is True
# Prompts for verification if 'prompt' is set to true
def RemoveFolder(folder_path, contents_only=False, prompt=True):
    if folder_path.exists() and folder_path.is_dir():
        if prompt:
            if contents_only:
                StatusMessage(f"\nYou are about to delete the contents of the folder at {folder_path}")
            else:
                StatusMessage(f"\nYou are about to delete the folder at {folder_path}")
            StatusMessage("Proceed? [Y/n]")
            user_response = input()
            if user_response != 'Y':
                return

        if contents_only:
            for item_path in folder_path.iterdir():
                if ('.gitignore' in item_path.parts) or ('.npmignore' in item_path.parts):
                    continue
                else:
                    StatusMessage(f"Removing: {item_path}")
                    rm_output = subprocess.run(f"rm -r {item_path}", stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
                    if rm_output.returncode != 0:
                        WarningMessage(f"Remove command completed with non-zero return code.\n\nSTDOUT:\n{rm_output.stdout}")
        else:
            rm_output = subprocess.run(f"rm -r {folder_path}", stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
            if rm_output.returncode != 0:
                WarningMessage(f"Remove command completed with non-zero return code.\n\nSTDOUT:\n{rm_output.stdout}")
    else:
        WarningMessage(f"No folder found at path: {folder_path}")
