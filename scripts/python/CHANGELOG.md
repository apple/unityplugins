# CHANGELOG
All notable changes to build.py and related python scripts will be noted here.

## [2.2.1] - 2024-04-11
### Updated
- Script has been updated to not sign native libraries by default now that Apple.Core has been updated to handle this step.
    - Passing no codesign identity hash is no longer an issue
    - Libraries can still be signed using the `-c` flag along with a passed string representing the codesign identity hash
    - The string `prompt` may be passed to `-c` to instigate the script's codesign workflow which will ask the user to select from a list of codesign identities on the system
