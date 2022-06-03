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
        public string PublicKeyUrl;
        internal IntPtr Signature;
        internal int SignatureLength;
        internal IntPtr Salt;
        internal int SaltLength;
        /// <summary>
        /// The signatureâ€™s creation date and time.
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// The verification signature data that GameKit generates.
        /// </summary>
        /// <returns></returns>
        public byte[] GetSignature()
        {
            var signature = new byte[SignatureLength];
            Marshal.Copy(Signature, signature, 0, SignatureLength);

            return signature;
        }

        /// <summary>
        /// A random NSString that GameKit uses to compute the hash and randomize it.
        /// </summary>
        /// <returns></returns>
        public byte[] GetSalt()
        {
            var salt = new byte[SaltLength];
            Marshal.Copy(Salt, salt, 0, SaltLength);

            return salt;
        }
    }
}