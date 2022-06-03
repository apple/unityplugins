#if UNITY_EDITOR_OSX
using System.Collections.Generic;
using System.IO;

namespace Apple.Core
{
    public class XcodeBuildCommand
    {
        private List<string> _parameters;

        public XcodeBuildCommand()
        {
            _parameters = new List<string>();
        }

        public XcodeBuildCommand WithScheme(string scheme)
        {
            _parameters.Add($"-scheme {scheme}");
            return this;
        }

        public XcodeBuildCommand WithProject(string projectPath)
        {
            _parameters.Add($"-project {projectPath}");
            return this;
        }

        public XcodeBuildCommand WithDestination(string destination)
        {
            _parameters.Add($"-destination {destination}");
            return this;
        }

        public XcodeBuildCommand WithSDK(string sdk)
        {
            _parameters.Add($"-sdk {sdk}");
            return this;
        }

        public XcodeBuildCommand WithConfiguration(string configuration)
        {
            _parameters.Add($"-configuration {configuration}");
            return this;
        }

        public XcodeBuildCommand WithArchivePath(string archivePath)
        {
            _parameters.Add($"-archivePath {archivePath}");
            return this;
        }

        public XcodeBuildCommand WithDevelopmentTeam(string developmentTeam)
        {
            _parameters.Add($"DEVELOPMENT_TEAM={developmentTeam}");
            return this;
        }

        public XcodeBuildCommand WithCodeSignStyle(string codeSignStyle)
        {
            _parameters.Add($"CODE_SIGN_STYLE={codeSignStyle}");
            return this;
        }

        public XcodeBuildCommand WithProvisionProfileSpecifierApp(string provisionProfileSpecifier)
        {
            _parameters.Add($"PROVISIONING_PROFILE_SPECIFIER_APP='{provisionProfileSpecifier}'");
            return this;
        }
        
        public XcodeBuildCommand WithProvisionProfileSpecifier(string provisionProfileSpecifier)
        {
            _parameters.Add($"PROVISIONING_PROFILE_SPECIFIER='{provisionProfileSpecifier}'");
            return this;
        }

        public XcodeBuildCommand WithCodeSigningIdentity(string codeSigningIdentity)
        {
            _parameters.Add($"CODE_SIGN_IDENTITY='{codeSigningIdentity}'");
            return this;
        }

        public XcodeBuildCommand WithEntitlementsFile(string entitlementsPath)
        {
            _parameters.Add($"CODE_SIGN_ENTITLEMENTS={entitlementsPath}");
            return this;
        }

        public XcodeBuildCommand WithExportOptionsPlist(string exportOptionsPlist)
        {
            _parameters.Add($"-exportOptionsPlist {exportOptionsPlist}");
            return this;
        }

        public XcodeBuildCommand WithKeychainPath(string keychainPath)
        {
            _parameters.Add($"OTHER_CODE_SIGN_FLAGS='--keychain={Path.GetFullPath(keychainPath)}'");
            return this;
        }

        public XcodeBuildCommand WithExportPath(string exportPath)
        {
            _parameters.Add($"-exportPath {exportPath}");
            return this;
        }

        public XcodeBuildCommand WithUseModernBuildSystem(bool useModernBuildSystem)
        {
            var param = useModernBuildSystem ? "YES" : "NO";
            _parameters.Add($"-UseModernBuildSystem={param}");

            return this;
        }

        public int Archive()
        {
            var command = $"xcodebuild {string.Join(" ", _parameters)} archive";
            return ShellCommandRunner.Run(command);
        }


        public int Export()
        {
            var command = $"xcodebuild -exportArchive {string.Join(" ", _parameters)}";
            return ShellCommandRunner.Run(command);
        }
    }
}
#endif