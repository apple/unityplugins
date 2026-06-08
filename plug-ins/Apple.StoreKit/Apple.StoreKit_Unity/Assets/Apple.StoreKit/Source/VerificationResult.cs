using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.StoreKit
{
    /// <summary>
    /// Generic verification result wrapper with strongly-typed payload access and JWS properties
    /// </summary>
    /// <typeparam name="T">The type of the signed payload (Transaction or AppTransaction)</typeparam>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public sealed class VerificationResult<T> : InteropReference where T : InteropReference
    {
        private T _cachedPayload;
        private readonly object _payloadLock = new object();
        private NSError _verificationError;
        private readonly object _errorLock = new object();

        internal VerificationResult(IntPtr pointer) : base(pointer) { }

        /// <summary>
        /// Whether the transaction was verified
        /// </summary>
        public bool IsVerified => Get<bool>();

        /// <summary>
        /// Gets the payload value as a string without verification
        /// </summary>
        public string UnsafePayloadValue => Get<string>();

        /// <summary>
        /// Gets the strongly-typed payload if verified
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is not verified</exception>
        public T SafePayload
        {
            get
            {
                if (!IsVerified)
                    throw new InvalidOperationException("Cannot access payload of unverified result. Use TryGetVerified() or UnsafePayload instead.");
                return GetPayload();
            }
        }

        /// <summary>
        /// Gets the strongly-typed payload without verification (returns payload whether verified or not)
        /// </summary>
        public T UnsafePayload => GetPayload();

        /// <summary>
        /// Gets the verification error if the result is unverified
        /// </summary>
        public NSError VerificationError
        {
            get
            {
                if (IsVerified)
                    return null;

                // Fast path without lock
                if (_verificationError != null)
                    return _verificationError;

                lock (_errorLock)
                {
                    if (_verificationError != null)
                        return _verificationError;

                    var errorPtr = Get<IntPtr>();
                    if (errorPtr == IntPtr.Zero)
                        return null;

                    _verificationError = new NSError(errorPtr);
                    return _verificationError;
                }
            }
        }

        #region JWS Properties

        /// <summary>
        /// The raw JSON Web Signature representation
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public string JwsRepresentation => Get<string>();

        /// <summary>
        /// The data for the header component of the JWS
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData HeaderData => Get<NSData>();

        /// <summary>
        /// The data for the payload component of the JWS
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData PayloadData => Get<NSData>();

        /// <summary>
        /// The data for the signature component of the JWS
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData SignatureData => Get<NSData>();

        /// <summary>
        /// The signature of the JWS as raw bytes (P256.Signing.ECDSASignature.rawRepresentation)
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData Signature => Get<NSData>();

        /// <summary>
        /// The component of the JWS that the signature is computed over
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData SignedData => Get<NSData>();

        /// <summary>
        /// The date the signature was generated
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public DateTime SignedDate => DateTimeOffset.FromUnixTimeSeconds(
            Get<long>()
        ).DateTime;

        /// <summary>
        /// A SHA-384 hash of AppStore.deviceVerificationID appended after deviceVerificationNonce
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public NSData DeviceVerification => Get<NSData>();

        /// <summary>
        /// The nonce used when computing deviceVerification
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public string DeviceVerificationNonce => Get<string>();

        #endregion

        #region Convenience Methods

        /// <summary>
        /// Attempts to get the verified payload
        /// </summary>
        /// <param name="payload">The verified payload if successful</param>
        /// <returns>True if verified, false otherwise</returns>
        public bool TryGetVerified(out T payload)
        {
            if (IsVerified)
            {
                payload = SafePayload;
                return true;
            }
            payload = null;
            return false;
        }

        /// <summary>
        /// Pattern matching helper for handling verified and unverified cases
        /// </summary>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="onVerified">Function to call if verified</param>
        /// <param name="onUnverified">Function to call if unverified</param>
        /// <returns>The result of the matching function</returns>
        public TResult Match<TResult>(
            Func<T, TResult> onVerified,
            Func<T, NSError, TResult> onUnverified)
        {
            return IsVerified
                ? onVerified(SafePayload)
                : onUnverified(UnsafePayload, VerificationError);
        }

        #endregion

        #region Private Helpers

        private T GetPayload()
        {
            if (_cachedPayload != null)
                return _cachedPayload;

            lock (_payloadLock)
            {
                if (_cachedPayload != null)
                    return _cachedPayload;

                var payloadPtr = Get<IntPtr>("GetPayloadPointer");
                _cachedPayload = typeof(T) switch
                {
                    Type t when t == typeof(Transaction) => (T)(object)new Transaction(payloadPtr),
                    Type t when t == typeof(AppTransaction) => (T)(object)new AppTransaction(payloadPtr),
                    Type t when t == typeof(RenewalInfo) => (T)(object)new RenewalInfo(payloadPtr),
                    _ => throw new NotSupportedException($"Type {typeof(T).Name} is not supported for VerificationResult"),
                };
                return _cachedPayload;
            }
        }

        private TReturn Get<TReturn>([System.Runtime.CompilerServices.CallerMemberName] string methodName = null)
        {
            if (typeof(T) == typeof(Transaction))
            {
                return methodName switch
                {
                    nameof(IsVerified) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_IsVerified(Pointer),
                    nameof(UnsafePayloadValue) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_UnsafePayloadValue(Pointer),
                    nameof(VerificationError) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_VerificationError(Pointer),
                    "GetPayloadPointer" => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_GetPayloadPointer(Pointer),
                    nameof(JwsRepresentation) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_GetJwsRepresentation(Pointer),
                    nameof(HeaderData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetHeaderData(Pointer)),
                    nameof(PayloadData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetPayloadData(Pointer)),
                    nameof(SignatureData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetSignatureData(Pointer)),
                    nameof(Signature) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetSignature(Pointer)),
                    nameof(SignedData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetSignedData(Pointer)),
                    nameof(SignedDate) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_GetSignedDate(Pointer),
                    nameof(DeviceVerification) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_Transaction_GetDeviceVerification(Pointer)),
                    nameof(DeviceVerificationNonce) => (TReturn)(object)VerificationResultInterop.VerificationResult_Transaction_GetDeviceVerificationNonce(Pointer),
                    _ => throw new NotSupportedException($"Method {methodName} not supported")
                };
            }
            else if (typeof(T) == typeof(AppTransaction))
            {
                return methodName switch
                {
                    nameof(IsVerified) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_IsVerified(Pointer),
                    nameof(UnsafePayloadValue) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_UnsafePayloadValue(Pointer),
                    nameof(VerificationError) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_VerificationError(Pointer),
                    "GetPayloadPointer" => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_GetPayloadPointer(Pointer),
                    nameof(JwsRepresentation) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_GetJwsRepresentation(Pointer),
                    nameof(HeaderData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetHeaderData(Pointer)),
                    nameof(PayloadData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetPayloadData(Pointer)),
                    nameof(SignatureData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetSignatureData(Pointer)),
                    nameof(Signature) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetSignature(Pointer)),
                    nameof(SignedData) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetSignedData(Pointer)),
                    nameof(SignedDate) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_GetSignedDate(Pointer),
                    nameof(DeviceVerification) => (TReturn)(object)new NSData(VerificationResultInterop.VerificationResult_AppTransaction_GetDeviceVerification(Pointer)),
                    nameof(DeviceVerificationNonce) => (TReturn)(object)VerificationResultInterop.VerificationResult_AppTransaction_GetDeviceVerificationNonce(Pointer),
                    _ => throw new NotSupportedException($"Method {methodName} not supported")
                };
            }
            else if (typeof(T) == typeof(RenewalInfo))
            {
                return methodName switch
                {
                    nameof(IsVerified) => (TReturn)(object)VerificationResultInterop.VerificationResult_RenewalInfo_IsVerified(Pointer),
                    nameof(VerificationError) => (TReturn)(object)VerificationResultInterop.VerificationResult_RenewalInfo_GetVerificationError(Pointer),
                    "GetPayloadPointer" => (TReturn)(object)VerificationResultInterop.VerificationResult_RenewalInfo_GetPayloadPointer(Pointer),
                    _ => throw new NotSupportedException($"Method {methodName} not supported for RenewalInfo VerificationResult")
                };
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported for VerificationResult");
            }
        }

        #endregion

        #region Disposal

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                if (typeof(T) == typeof(Transaction))
                {
                    VerificationResultInterop.VerificationResult_Transaction_Free(Pointer);
                }
                else if (typeof(T) == typeof(AppTransaction))
                {
                    VerificationResultInterop.VerificationResult_AppTransaction_Free(Pointer);
                }
                else if (typeof(T) == typeof(RenewalInfo))
                {
                    VerificationResultInterop.VerificationResult_RenewalInfo_Free(Pointer);
                }
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
    }

     #region Interop
        /// <summary>
        /// Interop functions for VerificationResult
        /// </summary>
        internal static class VerificationResultInterop
        {
            // Transaction verification result - Free function
            [DllImport(InteropUtility.DLLName)]
            public static extern void VerificationResult_Transaction_Free(IntPtr pointer);

            // Transaction verification result - common functions
            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool VerificationResult_Transaction_IsVerified(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_Transaction_UnsafePayloadValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_VerificationError(IntPtr pointer);

            // Transaction verification result - JWS functions
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetPayloadPointer(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_Transaction_GetJwsRepresentation(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetHeaderData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetPayloadData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetSignatureData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetSignature(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetSignedData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long VerificationResult_Transaction_GetSignedDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_Transaction_GetDeviceVerification(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_Transaction_GetDeviceVerificationNonce(IntPtr pointer);

            // AppTransaction verification result - Free function
            [DllImport(InteropUtility.DLLName)]
            public static extern void VerificationResult_AppTransaction_Free(IntPtr pointer);

            // AppTransaction verification result - common functions
            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool VerificationResult_AppTransaction_IsVerified(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_AppTransaction_UnsafePayloadValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_VerificationError(IntPtr pointer);

            // AppTransaction verification result - JWS functions
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetPayloadPointer(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_AppTransaction_GetJwsRepresentation(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetHeaderData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetPayloadData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetSignatureData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetSignature(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetSignedData(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long VerificationResult_AppTransaction_GetSignedDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_AppTransaction_GetDeviceVerification(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string VerificationResult_AppTransaction_GetDeviceVerificationNonce(IntPtr pointer);

            // RenewalInfo verification result
            [DllImport(InteropUtility.DLLName)]
            public static extern void VerificationResult_RenewalInfo_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool VerificationResult_RenewalInfo_IsVerified(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_RenewalInfo_GetPayloadPointer(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr VerificationResult_RenewalInfo_GetVerificationError(IntPtr pointer);
        }
        #endregion
}
