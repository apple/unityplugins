#if UNITY_EDITOR_OSX

using System.IO;

namespace Apple.Core
{
    public class KeychainCommand
    {
        private string _keychainPath;
        private string _keychainPassword;
        public bool _verbose;

        public KeychainCommand WithKeychainPath(string keychainPath)
        {
            _keychainPath = Path.GetFullPath(keychainPath);
            return this;
        }

        public KeychainCommand WithKeychainPassword(string keychainPassword)
        {
            _keychainPassword = keychainPassword;
            return this;
        }

        public KeychainCommand WithVerbose(bool verbose)
        {
            _verbose = verbose;
            return this;
        }

        public int Unlock()
        {
            var command = $"(security {GetVerboseString()} unlock-keychain -p {_keychainPassword} {_keychainPath} && security {GetVerboseString()} set-keychain-settings -lut 7200 {_keychainPath})";

            return ShellCommandRunner.Run(command, new string[] { _keychainPassword });
        }

        private string GetVerboseString()
        {
            if(_verbose)
            {
                return "-v";
            }

            return string.Empty;
        }

        public int AddGenericPassword(string account, string keychainItemName, string password)
        {
            var command = $"security add-generic-password -a '{account}' -s '{keychainItemName}' -w '{password}'";
            return ShellCommandRunner.Run(command, new []{password}, out _, out _);
        }
        
        public int FindGenericPassword(string account, string keychainItemName, out string password)
        {
            var command = $"security find-generic-password -a '{account}' -s '{keychainItemName}' -w";
            return ShellCommandRunner.Run(command, null, out password, out _);
        }

        public int DeleteGenericPassword(string account, string keychainItemName)
        {
            var command = $"security delete-generic-password -a '{account}' -s '{keychainItemName}'";
            return ShellCommandRunner.Run(command, null, out _, out _);
        }

        public int SetAsDefaultKeychain()
        {
            var command = $"(security {GetVerboseString()} list-keychain -d user -s {_keychainPath} && security {GetVerboseString()} default-keychain -d user -s '{_keychainPath}')";
            return ShellCommandRunner.Run(command);
        }
        
        public int RestoreLoginDefaultKeychain()
        {
            var command = $"(security {GetVerboseString()} list-keychain -d user -s login.keychain && security {GetVerboseString()} default-keychain -d user -s login.keychain)";
            return ShellCommandRunner.Run(command);
        }
    }
}
#endif