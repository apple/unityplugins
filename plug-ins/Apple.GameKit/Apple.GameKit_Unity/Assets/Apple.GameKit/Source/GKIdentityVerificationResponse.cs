using System;
using Apple.Core.Runtime;

namespace Apple.GameKit.Players
{
    public class GKIdentityVerificationResponse
    {
        /// <summary>
        /// The URL for the public encryption key.
        /// </summary>
        public string PublicKeyUrl { get; set; }

        /// <summary>
        /// The verification signature data that GameKit generates.
        /// </summary>
        public NSData Signature { get; set; }
        public byte[] GetSignature() => Signature.Bytes;

        /// <summary>
        /// A random NSString that GameKit uses to compute the hash and randomize it.
        /// </summary>
        public NSData Salt { get; set; }
        public byte[] GetSalt() => Salt.Bytes;

        /// <summary>
        /// The signature's creation date and time.
        /// </summary>
        public UInt64 Timestamp;
    }
}