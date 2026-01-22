#! /usr/bin/env python3
# Requirements: python3

"""
Assembly Definition Platform Validator

This module provides functionality to validate and fix Unity assembly definition (.asmdef) files
to ensure they only reference platforms that are supported by the target Unity version.

This solves the issue where Unity 2021 doesn't support visionOS (added in 2022), but asmdef
files include "VisionOS" in includePlatforms, causing compilation errors when installed as
read-only packages via Unity Package Manager.
"""

import json
import shutil
from pathlib import Path
from typing import Dict, List, Set

class AsmdefPlatformValidator:
    """Validates and fixes assembly definition files based on supported platforms."""

    # Mapping from build platform IDs to Unity asmdef platform names
    PLATFORM_MAPPING = {
        "iOS": "iOS",
        "iOS_Simulator": "iOS",  # iOS Simulator uses same platform in asmdef
        "tvOS": "tvOS",
        "tvOS_Simulator": "tvOS",  # tvOS Simulator uses same platform in asmdef
        "macOS": "macOSStandalone",
        "visionOS": "VisionOS",
        "visionOS_Simulator": "VisionOS"  # visionOS Simulator uses same platform in asmdef
    }

    def __init__(self, printer=None):
        """
        Initialize the validator.

        Args:
            printer: Optional printer object for logging messages
        """
        self.printer = printer
        self.backup_files = []  # Track backed up files for restoration

    def _log(self, message: str, indent: int = 0):
        """Log a message using the printer if available."""
        if self.printer:
            self.printer.Message(" " * (indent * 2) + message)
        else:
            print(" " * (indent * 2) + message)

    def _log_status(self, message: str, indent: int = 0):
        """Log a status message using the printer if available."""
        if self.printer:
            self.printer.StatusMessage(" " * (indent * 2) + message)
        else:
            print(" " * (indent * 2) + message)

    def _log_warning(self, message: str, indent: int = 0):
        """Log a warning message using the printer if available."""
        if self.printer:
            self.printer.WarningMessage(" " * (indent * 2) + message)
        else:
            print(" " * (indent * 2) + "WARNING: " + message)

    def get_supported_asmdef_platforms(self, built_platforms: Dict[str, bool]) -> Set[str]:
        """
        Convert build platform flags to Unity asmdef platform names.

        Args:
            built_platforms: Dictionary of platform IDs to boolean (True if built)

        Returns:
            Set of Unity asmdef platform names that were built
        """
        supported_platforms = set()

        # Always include Editor platform
        supported_platforms.add("Editor")

        # Map build platforms to asmdef platforms
        for platform_id, was_built in built_platforms.items():
            if was_built and platform_id in self.PLATFORM_MAPPING:
                supported_platforms.add(self.PLATFORM_MAPPING[platform_id])

        return supported_platforms

    def validate_and_fix_asmdef(self, asmdef_path: Path, supported_platforms: Set[str]) -> bool:
        """
        Validate and fix an assembly definition file.

        Args:
            asmdef_path: Path to the .asmdef file
            supported_platforms: Set of supported platform names

        Returns:
            True if the file was modified, False otherwise
        """
        try:
            # Read the asmdef file
            with open(asmdef_path, 'r') as f:
                asmdef_data = json.load(f)

            # Check if includePlatforms exists and has entries
            if 'includePlatforms' not in asmdef_data or not asmdef_data['includePlatforms']:
                # No platform restrictions, so this is fine
                return False

            original_platforms = set(asmdef_data['includePlatforms'])

            # Find unsupported platforms
            unsupported_platforms = original_platforms - supported_platforms

            if not unsupported_platforms:
                # All platforms are supported
                return False

            self._log_warning(f"Found unsupported platform(s) in {asmdef_path.name}: {', '.join(sorted(unsupported_platforms))}", 1)

            # Create backup before modifying
            backup_path = asmdef_path.with_suffix('.asmdef.backup')
            shutil.copy2(asmdef_path, backup_path)
            self.backup_files.append((asmdef_path, backup_path))

            # Remove unsupported platforms
            fixed_platforms = sorted(original_platforms & supported_platforms)
            asmdef_data['includePlatforms'] = fixed_platforms

            # Write back to file with pretty formatting
            with open(asmdef_path, 'w') as f:
                json.dump(asmdef_data, f, indent=4)
                f.write('\n')  # Add trailing newline

            self._log_status(f"Fixed {asmdef_path.name} - removed: {', '.join(sorted(unsupported_platforms))}", 1)

            return True

        except Exception as ex:
            self._log_warning(f"Failed to process {asmdef_path}: {ex}")
            return False

    def restore_asmdef_files(self) -> None:
        """
        Restore all backed up asmdef files to their original state.

        This should be called after packaging is complete to avoid
        leaving modified asmdef files in the source tree.
        """
        if not self.backup_files:
            return

        self._log_status(f"Restoring {len(self.backup_files)} asmdef file(s) to original state")

        for original_path, backup_path in self.backup_files:
            try:
                if backup_path.exists():
                    shutil.move(str(backup_path), str(original_path))
                    self._log_status(f"Restored {original_path.name}", 1)
            except Exception as ex:
                self._log_warning(f"Failed to restore {original_path}: {ex}", 1)

        # Clear the backup list
        self.backup_files.clear()

    def process_plugin_asmdefs(self, plugin_path: Path, supported_platforms: Set[str]) -> int:
        """
        Process all .asmdef files in a plugin directory.

        Args:
            plugin_path: Path to the plugin root directory
            supported_platforms: Set of supported platform names

        Returns:
            Number of files that were modified
        """
        asmdef_files = list(plugin_path.glob('**/*.asmdef'))

        if not asmdef_files:
            return 0

        modified_count = 0
        for asmdef_path in asmdef_files:
            if self.validate_and_fix_asmdef(asmdef_path, supported_platforms):
                modified_count += 1

        return modified_count

    def validate_plugin_before_packaging(self, plugin_path: Path, built_platforms: Dict[str, bool]) -> None:
        """
        Validate and fix all asmdef files in a plugin before packaging.

        This should be called before the tar packaging step to ensure that the
        packaged plugin only references platforms that were actually built.

        Args:
            plugin_path: Path to the plugin Unity project
            built_platforms: Dictionary of platform IDs to boolean (True if built)
        """
        supported_platforms = self.get_supported_asmdef_platforms(built_platforms)

        self._log(f"Validating assembly definitions for: {plugin_path.name}")
        self._log(f"Supported platforms: {', '.join(sorted(supported_platforms))}", 1)

        # Find the Unity package directory (where package.json lives)
        package_json_files = list(plugin_path.glob('**/package.json'))

        for package_json_path in package_json_files:
            # Skip package cache entries
            if 'PackageCache' in str(package_json_path):
                continue

            package_root = package_json_path.parent
            modified_count = self.process_plugin_asmdefs(package_root, supported_platforms)

            if modified_count > 0:
                self._log_status(f"Fixed {modified_count} assembly definition file(s)")
            else:
                self._log_status("All assembly definitions are valid")

            break


# Standalone testing function
if __name__ == '__main__':
    import sys

    if len(sys.argv) < 2:
        print("Usage: python upi_asmdef_validator.py <plugin_path>")
        sys.exit(1)

    plugin_path = Path(sys.argv[1])
    if not plugin_path.exists():
        print(f"Error: Path does not exist: {plugin_path}")
        sys.exit(1)

    # Test with all platforms for demonstration
    test_platforms = {
        "iOS": True,
        "macOS": True,
        "tvOS": True,
        "visionOS": False  # Simulate Unity 2021 (no visionOS)
    }

    validator = AsmdefPlatformValidator()
    validator.validate_plugin_before_packaging(plugin_path, test_platforms)
