#! /usr/bin/env python3
# Requirements: python3

import scripts.upi_utility as utility

from collections.abc import Callable
from scripts.upi_utility import Printer

#--------------
# Build Helpers

# Invokes 'xcodebuild -version' and parses output for the associated Xcode version and build number
def GetToolchainVersions() -> tuple[str, str]:
    xcode_version = ""
    build_number = ""
    version_output = utility.RunCommand(["xcodebuild", "-version"])
    output_lines = version_output.stdout.split('\n')
    for line in output_lines:
        if line.startswith("Xcode"):
            xcode_version = line.split(' ')[1]
        elif line.startswith("Build"):
            build_number = line.split(' ')[2]
    
    return (xcode_version, build_number)

#-------------
# Code Signing

# Returns a dictionary of formatted output from a command line invocation of 'security find-identity -v -p codesigning', or an empty dictionary if no identities are found.
#
# This command returns a multi-line response which lists each identity found along with an additional line which summarizes the number of found identities
#
# Example output:
#   1) HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH "Title of identity: Developer Name (XXXXXXXXXX)"
#      1 valid identity found
#
# This method processes that output and returns a dictionary which contains the hash mapped to the identity name
# e.g.
#  {'HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH' : '"Title of identity: Developer Name (XXXXXXXXXX)"', ...}
#
# Organizing the output dictionary in this manner provides an easy means of listing the full identity and retreiving just the hash without further parsing.
def GetCodesignIdentities() -> dict[str, str]:
    security_command_output = utility.RunCommand(["security", "find-identity", "-v", "-p",  "codesigning"])
    output_lines = security_command_output.stdout.strip().split("\n")
        
    codesign_identity_table = dict()
    num_lines = len(output_lines)
    if num_lines > 1:
        for line_index in range(num_lines - 1):
            # Remove leading/trailing white space
            curr_output_line = output_lines[line_index].strip()
            
            # Hash substring starts after the first space and runs to the second space
            hash_start_index = curr_output_line.find(' ') + 1
            hash_end_index = curr_output_line.find(' ', hash_start_index)
            hash = curr_output_line[hash_start_index : hash_end_index]

            # Codesign identity 'name' string consumes remaining portion of output string
            identity_name = curr_output_line[hash_end_index + 1:]
            
            codesign_identity_table[hash] = identity_name

    return codesign_identity_table

# Performs codesigning on supplied object, such as a .framework or .bundle, using the supplied codesign identity hash.
#   See: GetCodesignIdentities for querying codesign identities on the system.
def Codesign(printer : Printer, signable_object_path : str, hash : str, logWithContext : Callable[[str, str], None] = None) -> bool:
    codesign_command = ["codesign", "--force", "--sign", hash, "--timestamp=none", "--preserve-metadata=identifier,entitlements,flags", "--generate-entitlement-der", f"{signable_object_path}"]
    
    if logWithContext == None:
        printer.MessageWithContext("Target object: ", signable_object_path)
        printer.MessageWithContext("Codesign command: ", f"{' '.join(codesign_command)}")
    else:
        logWithContext("Target object: ", signable_object_path)
        logWithContext("Codesign command: ", f"{' '.join(codesign_command)}")

    codesign_command_output = utility.RunCommand(codesign_command)
    if codesign_command_output.returncode != 0:
        printer.ErrorMessage("Failed to perform codesign.")
        printer.Message("Command Response:" 
                        f"\n{codesign_command_output.stdout}")
        return False
    else:
        return True
    
# Method prompts the user before identifying a codesign identity to use for signing newly compiled native plug-in libraries
def PromptForCodesignIdentity(printer : Printer) -> str:
    printer.WarningMessage("No codesign identity provided.", "\n")
    printer.InfoMessage("Recent versions of Unity require that native plug-in libraries are codesigned or they will not be loaded by the Editor or Player runtime.")
    printer.Message(f"For more information about code signing, please see: {Printer.Bold('https://developer.apple.com/library/archive/documentation/Security/Conceptual/CodeSigningGuide/Introduction/Introduction.html')}", printer.Indent(1))
    
    if not utility.BooleanPrompt(printer, "Would you like the script to code sign the compiled native plug-in libraries?"):
        printer.Message("User opted out of code signing. Compiled libraries will not be signed and may not be loaded by Unity.")
        return ""
    
    codesign_identities = GetCodesignIdentities()
    if len(codesign_identities) == 0:
        printer.WarningMessage("No codesign identities found. Compiled libraries will not be signed and may not be loaded by Unity.")
        return ""
    
    elif len(codesign_identities) == 1:
        id_hash, id_name = codesign_identities.items()[0]
        printer.MessageWithContext("Using codesign identity: ", f"{id_hash} {id_name}", "\n")
        return id_hash

    else:
        id_hash, id_name = utility.SelectionPrompt(printer, "Multiple codesign identities found. Please select an identity to use for signing:", [(h, n) for (h, n) in codesign_identities.items()], display_func=lambda hn: f"{hn[0]} {hn[1]}")
        printer.MessageWithContext("User selected codesign identity: ", f"{id_hash} {id_name}", "\n")
        return id_hash
