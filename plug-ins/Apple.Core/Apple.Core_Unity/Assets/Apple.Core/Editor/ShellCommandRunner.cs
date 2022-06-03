#if UNITY_EDITOR_OSX
using System;
using UnityEngine;

namespace Apple.Core
{
    public static class ShellCommandRunner
    {
        public static bool DisplayUnityProgressBar = true;
        
        /// <summary>
        /// Executies the shell command and masks any values
        /// specified in maskValues with [Masked] from logs.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="maskValues"></param>
        /// <returns></returns>
        public static int Run(string command, string[] maskValues = null)
        {
            var exitCode = Run(command, maskValues, out var standardOutput, out var standardError);

            if (!string.IsNullOrEmpty(standardOutput))
            {
                Debug.Log(SanitizeForLog(standardOutput, maskValues));
            }
            
            if (!string.IsNullOrEmpty(standardError))
            {
                Debug.LogError(SanitizeForLog(standardError, maskValues));
            }

            return exitCode;
        }

        public static int Run(string command, string[] maskValues, out string standardOutput, out string standardError)
        {
            standardOutput = null;
            standardError = null;
            
            Debug.Log($"ShellCommandRunner: Running => {SanitizeForLog(command, maskValues)}");

            if(!Application.isBatchMode && DisplayUnityProgressBar)
            {
                UnityEditor.EditorUtility.DisplayProgressBar("ShellCommandRunner", SanitizeForLog(command, maskValues), .25f);
            }

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = @"/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();
            var error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            // Log output...
            if (!string.IsNullOrEmpty(output))
            {
                standardOutput = output;
            }

            // Log errors...
            if(!string.IsNullOrEmpty(error))
            {
                standardError = output;
            }

            if (!Application.isBatchMode && DisplayUnityProgressBar)
            {
                UnityEditor.EditorUtility.ClearProgressBar();
            }
            
            return proc.ExitCode;
        }

        private static string SanitizeForLog(string outputLog, string[] maskValues)
        {
            if (maskValues == null)
                return outputLog;

            var sanitizedCommand = outputLog;

            foreach(var value in maskValues)
            {
                if (string.IsNullOrEmpty(value))
                    continue;

                sanitizedCommand = sanitizedCommand.Replace(value, "[Masked]");
            }

            return sanitizedCommand;
        }
    }
}
#endif