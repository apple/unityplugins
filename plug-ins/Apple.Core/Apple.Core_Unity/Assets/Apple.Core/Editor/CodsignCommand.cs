#if UNITY_EDITOR_OSX
using System.Collections.Generic;
using System.IO;

namespace Apple.Core
{
    public class CodsignCommand
    {
        private List<string> _parameters;
        private List<string> _targets;

        public CodsignCommand()
        {
            _parameters = new List<string>();
            _targets = new List<string>();
        }

        public CodsignCommand WithForce()
        {
            _parameters.Add("--force");
            return this;
        }

        public CodsignCommand WithVerify()
        {
            _parameters.Add("--verify");
            return this;
        }

        public CodsignCommand WithDeep()
        {
            _parameters.Add("--deep");
            return this;
        }

        public CodsignCommand WithVerbose()
        {
            _parameters.Add("--verbose");
            return this;
        }

        public CodsignCommand WithKeychainPath(string keychainPath)
        {
            _parameters.Add($"--keychain {Path.GetFullPath(keychainPath)}");
            return this;
        }

        public CodsignCommand WithPreserveMetaData(string metaData)
        {
            _parameters.Add($"--preserve-metadata={metaData}");
            return this;
        }

        public CodsignCommand WithSigningIdentity(string signingIdentityName)
        {
            _parameters.Add($"--sign '{signingIdentityName}'");
            return this;
        }

        public CodsignCommand WithEntitlementsPath(string entitlementsPath)
        {
            _parameters.Add($"--entitlements {entitlementsPath}");
            return this;
        }

        public CodsignCommand WithTarget(string targetPath)
        {
            _targets.Add(targetPath);
            return this;
        }

        public CodsignCommand WithTargets(IEnumerable<string> targetPaths)
        {
            _targets.AddRange(targetPaths);
            return this;
        }

        public int Run()
        {
            var commands = new List<string>();

            foreach(var target in _targets)
            {
                var command = $"codesign {string.Join(" ", _parameters)} {target}";
                commands.Add(command);
            }

            var compoundCommand = string.Join(" && ", commands);

            return ShellCommandRunner.Run(compoundCommand);
        }
    }
}
#endif