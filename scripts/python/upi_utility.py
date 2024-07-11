#! /usr/bin/env python3
# Requirements: python3

import subprocess

from pathlib import Path

from collections.abc import Callable

#------------------------------
# Terminal Prompt Color Helpers

# Define colors and modifiers
class PromptColor:
    # Foreground - Text Colors

    # Dark
    BLACK   = "\033[30m"
    RED     = "\033[31m"
    GREEN   = "\033[32m"
    YELLOW  = "\033[33m"
    BLUE    = "\033[34m"
    MAGENTA = "\033[35m"
    CYAN    = "\033[36m"
    WHITE   = "\033[37m"

    # Bright
    BRIGHT_BLACK   = "\033[90m"
    BRIGHT_RED     = "\033[91m"
    BRIGHT_GREEN   = "\033[92m"
    BRIGHT_YELLOW  = "\033[93m"
    BRIGHT_BLUE    = "\033[94m"
    BRIGHT_MAGENTA = "\033[95m"
    BRIGHT_CYAN    = "\033[96m"
    BRIGHT_WHITE   = "\033[97m"

    # Background - Behind Text Colors
    
    #Dark
    BG_BLACK   = "\033[40m"
    BG_RED     = "\033[41m"
    BG_GREEN   = "\033[42m"
    BG_YELLOW  = "\033[43m"
    BG_BLUE    = "\033[44m"
    BG_MAGENTA = "\033[45m"
    BG_CYAN    = "\033[46m"
    BG_WHITE   = "\033[47m"

    #Bright
    BG_BRIGHT_BLACK   = "\033[100m"
    BG_BRIGHT_RED     = "\033[101m"
    BG_BRIGHT_GREEN   = "\033[102m"
    BG_BRIGHT_YELLOW  = "\033[103m"
    BG_BRIGHT_BLUE    = "\033[104m"
    BG_BRIGHT_MAGENTA = "\033[105m"
    BG_BRIGHT_CYAN    = "\033[106m"
    BG_BRIGHT_WHITE   = "\033[107m"

    # Modifiers
    BOLD      = "\033[01m"
    ITALIC    = "\033[03m"
    UNDERLINE = "\033[04m"

    # Reset to  default terminal settings
    RESET = "\033[00m"

    # Sentinel
    NONE = ""

class PromptTheme:
    def __init__(self) -> None:
        self.standard_output_color = PromptColor.NONE
        self.section_heading_color = PromptColor.NONE
        self.context_color = PromptColor.NONE
        self.status_color = PromptColor.NONE

        self.user_input_bg_color = PromptColor.NONE
        self.user_input_color = PromptColor.NONE

        self.error_bg_color = PromptColor.NONE
        self.error_color = PromptColor.NONE

        self.warning_bg_color = PromptColor.NONE
        self.warning_color = PromptColor.NONE

        self.info_bg_color = PromptColor.NONE
        self.info_color = PromptColor.NONE

        self.indent_string = ""

class Printer:
    def __init__(self, theme : PromptTheme) -> None:
        self.theme = theme

    # -------------
    # Class Methods

    # Decorate a string with selected color
    def Decorate(text : str, color : PromptColor) -> str:
        return f"{color}{text}{PromptColor.RESET}" if len(str(text)) > 0 else ""
    
    # Decoate with multiple colors
    def MultiDecorate(text : str, *colors : str) -> str:
        if len(str(text)) == 0:
            return ""
        
        color_format_string = ""
        for color in colors:
            color_format_string += color

        return f"{color_format_string}{text}{PromptColor.RESET}"

    # Return a string decorated wiht BOLD formatting.
    def Bold(text : str) -> str:
        return Printer.Decorate(text, PromptColor.BOLD)
    
    # Sometimes you want a blank newline
    def Newline() -> None:
        print("")

    # ----------------
    # Instance Methods

    # Prints an error message with highlighted tag
    def ErrorMessage(self, message : str, prefix : str = "\n") -> None:
        print(f"{prefix}<{Printer.MultiDecorate('Error', self.theme.error_bg_color, self.theme.error_color)}>: {message}")
            
    # Prints an warning message with highlighted tag
    def WarningMessage(self, message : str, prefix : str = "\n") -> None:
        print(f"{prefix}<{Printer.MultiDecorate('Warning', self.theme.warning_bg_color, self.theme.warning_color)}>: {message}")

    # Prints an Info message with highlighted tag
    def InfoMessage(self, message : str, prefix : str = "") -> None:
        print(f"{prefix}<{Printer.MultiDecorate('Info', self.theme.info_bg_color, self.theme.info_color)}>: {message}")

    # Prints a standard message
    def Message(self, message : str, prefix : str = "") -> None:
        print(f"{prefix}{Printer.Decorate(message, self.theme.standard_output_color)}")

    # Prints a standard message with additional highlighted context
    def MessageWithContext(self, message : str, context : str = "", prefix : str = "") -> None:
        print(f"{prefix}{Printer.Decorate(message, self.theme.standard_output_color)}{Printer.Decorate(context, self.theme.context_color)}")
        
    # Prints a status message
    def StatusMessage(self, message : str, prefix : str = "") -> None:
        print(f"{prefix}{Printer.Decorate(message, self.theme.status_color)}")

    # Prints a status message with additional highlighted context
    def StatusMessageWithContext(self, message : str, context : str = "", prefix : str = "") -> None:
        print(f"{prefix}{Printer.Decorate(message, self.theme.status_color)}{Printer.Decorate(context, self.theme.context_color)}")

    def SectionHeading(self, heading : str) -> None:
        print(f"\n{Printer.Decorate(heading, self.theme.section_heading_color)}")
        self.Message('-' * len(heading))

    # Get a context message formatted with the theme's context color
    def Context(self, context : str) -> str:
        return Printer.Decorate(context, self.theme.context_color)

    # Helper to generate consistent indentation in output
    def Indent(self, level : int = 1) -> str:
        return self.theme.indent_string * level

#---------------
# Prompt Helpers

# Display a prompt defined by sPrompt returning true if the user input is 'Y', false if 'n', and repeatedly asks otherwise.
def BooleanPrompt(printer : Printer, prompt : str) -> bool:
    print(f"\n{Printer.MultiDecorate(prompt, printer.theme.user_input_bg_color, printer.theme.user_input_color)} {Printer.Decorate('[Y/n]', printer.theme.standard_output_color)}")
    
    while True:
        user_response = input()
        if user_response == 'Y':
            return True
        elif user_response == 'n':    
            return False
        else:
            printer.Message(f"Please answer {Printer.Bold('Y')} for yes or {Printer.Bold('n')} for no.")
            continue

# Displays a prompt which offers user selection from list of string options
#  Validates input is numerical and is on the correct range.
def SelectionPrompt(printer: Printer, prompt : str, options : list[any], display_func : Callable[[any], str] = lambda x : f"{x}") -> any:
    num_options = len(options)

    if num_options == 0:
        return None
    
    if num_options == 1:
        return options[0]

    print(f"\n{Printer.MultiDecorate(prompt, printer.theme.user_input_bg_color, printer.theme.user_input_color)}")
    
    curr_option_index = 0
    for option in options:
        printer.Message(f"{printer.Indent(1)}{curr_option_index}) {display_func(option)}")
        curr_option_index += 1
    
    while True:
        user_response = input()
        selection = -1
        try:
            selection = int(user_response)
        except ValueError:
            printer.Message(f"You entered: {Printer.Bold(user_response)}")
            printer.Message(f"\n  Please enter a number on the range [0 - {curr_option_index - 1}]")
            continue
        
        if selection >= 0 and selection < curr_option_index:
            return options[selection]
        else:
            printer.Message(f"You chose and out of range index: {Printer.Bold(user_response)}")
            printer.Message(f"\n  Please enter a number on the range [0 - {curr_option_index - 1}]")
            continue

#-------------------
# Subprocess Helpers

# Helper runs command with standard set of arguments, returning result from underlying subprocess.run() call.
def RunCommand(command : list[str]) -> subprocess.CompletedProcess[str]:
    return subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)

#-------------------------
# Folder Structure Helpers

# Removes data within the folder at 'folder_path'
# Keeps folder in place but removes all contents, except .gitignore and .npmignore files, when 'contents_only' is True
# Prompts for verification if 'prompt' is set to true
def RemoveFolder(folder_path : Path, printer : Printer, contents_only : bool = False, prompt : bool = True) -> None:
    if folder_path.exists() and folder_path.is_dir():
        if prompt:
            if contents_only:
                printer.InfoMessage(f"You are about to delete the contents of the folder at {folder_path}")
            else:
                printer.InfoMessage(f"You are about to delete the folder at {folder_path}")

            if not BooleanPrompt(printer, "Proceed?"):
                return

        if contents_only:
            for item_path in folder_path.iterdir():
                if ('.gitignore' in item_path.parts) or ('.npmignore' in item_path.parts):
                    continue
                else:
                    printer.StatusMessage(f"Removing: {item_path}")
                    rm_output = RunCommand(["rm", "-f", item_path])
                    if rm_output.returncode != 0:
                        printer.WarningMessage(f"Remove command completed with non-zero return code.\n\nSTDOUT:\n{rm_output.stdout}")
        else:
            rm_output = RunCommand(["rm", "-rf", folder_path])
            if rm_output.returncode != 0:
                printer.WarningMessage(f"Remove command completed with non-zero return code.\n\nSTDOUT:\n{rm_output.stdout}")
    else:
        printer.WarningMessage(f"No folder found at path: {folder_path}")
