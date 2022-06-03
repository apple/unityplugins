#if UNITY_EDITOR_OSX

using System.Collections.Generic;
using System.IO;

namespace Apple.Core
{
    public class XcrunCommand
    {
        private List<string> _parameters;
        private List<string> _maskedParameters;

        public XcrunCommand()
        {
            _parameters = new List<string>();
            _maskedParameters = new List<string>();
        }

        public XcrunCommand WithCredentials(string username, string password)
        {
            _parameters.Add($"--username '{username}' --password '{password}'");
            _maskedParameters.Add(username);
            _maskedParameters.Add(password);
            return this;
        }

        public XcrunCommand WithType(string type)
        {
            _parameters.Add($"--type {type}");
            return this;
        }

        public XcrunCommand WithFile(string filePath)
        {
            _parameters.Add($"--file {Path.GetFullPath(filePath)}");
            return this;
        }

        public int ValidateApp()
        {
            var command = $"xcrun altool --validate-app {string.Join(" ", _parameters)}";

            return ShellCommandRunner.Run(command, _maskedParameters.ToArray());
        }

        public int UploadApp()
        {
            var command = $"xcrun altool --upload-app {string.Join(" ", _parameters)}";

            return ShellCommandRunner.Run(command, _maskedParameters.ToArray());
        }
    }
}
#endif