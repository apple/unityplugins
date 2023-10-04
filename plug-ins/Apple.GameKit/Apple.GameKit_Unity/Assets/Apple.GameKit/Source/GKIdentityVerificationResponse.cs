using System;
using System.Runtime.InteropServices;

namespace Apple.GameKit.Players
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GKIdentityVerificationResponse
    {
        /// <summary>
        /// The URL for the public encryption key.
        /// </summary>
        //public string PublicKeyUrl;
        public string PublicKeyUrl;

        /// <summary>
        /// The verification signature data that GameKit generates.
        /// </summary>
        public byte[] Signature;
        
        /// <summary>
        /// A random NSString that GameKit uses to compute the hash and randomize it.
        /// </summary>
        public byte[] Salt;

        /// <summary>
        /// The signatureâ€™s creation date and time.
        /// </summary>
        public ulong Timestamp;

        internal GKIdentityVerificationResponse(ulong timestamp,
            IntPtr publicKeyUrl, int publicKeyUrlLength, 
            IntPtr signature, int signatureLength, 
            IntPtr salt, int saltLength)
        {
            Timestamp = timestamp;

            var publicKeyUrlBytes = new byte[publicKeyUrlLength];
            Marshal.Copy(publicKeyUrl, publicKeyUrlBytes, 0, publicKeyUrlLength);
            PublicKeyUrl = System.Text.Encoding.UTF8.GetString(publicKeyUrlBytes);
            
            Signature = new byte[signatureLength];
            Marshal.Copy(signature, Signature, 0, signatureLength);

            Salt = new byte[saltLength];
            Marshal.Copy(salt, Salt, 0, saltLength);

        }
        
    }
}