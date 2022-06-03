#if UNITY_EDITOR_OSX
using System.Collections.Generic;
using System.IO;

namespace Apple.Core
{
    public class ProductBuildCommand
    {
        private List<string> _parameters;

        public ProductBuildCommand()
        {
            _parameters = new List<string>();
        }

        public ProductBuildCommand WithComponent(string component, string installPath = "/Applications")
        {
            _parameters.Add($"--component {component} {installPath}");
            return this;
        }

        public ProductBuildCommand WithSigningIdentity(string codeSigningIdentifier)
        {
            _parameters.Add($"--sign '{codeSigningIdentifier}'");
            return this;
        }

        public ProductBuildCommand WithKeychainPath(string keychainPath)
        {
            _parameters.Add($"--keychain '{Path.GetFullPath(keychainPath)}'");
            return this;
        }

        public ProductBuildCommand WithVersion(string version)
        {
            _parameters.Add($"--version '{version}'");
            return this;
        }

        public int Run(string pkgOutputPath)
        {
            var command = $"productbuild {string.Join(" ", _parameters)} {pkgOutputPath}";

            return ShellCommandRunner.Run(command);
        }
    }
}
#endif