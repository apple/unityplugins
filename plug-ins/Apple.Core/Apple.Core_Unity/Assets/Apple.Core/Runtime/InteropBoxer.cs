using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// Box and unbox values to/from NSObject
    /// Types of objects supported:
    /// * NSNull: to/from null
    /// * NSObject: no-op
    /// * NSObject subclass: down/up cast
    /// * NSString: to/from string
    /// * NSNumber: to/from bool, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double
    /// * NSArray: to/from C# NSArray<>
    /// * NSMutableArray: to/from C# NSMutableArray<>
    /// * NSDictionary: to/from C# NSDictionary<,>
    /// * NSMutableDictionary: to/from C# NSMutableDictionary<,>
    /// </summary>
    public static class InteropBoxer
    {
        /// <summary>
        /// Try to box a <paramref name="payload"/> of type <paramref name="payloadType"/> into an NSObject <paramref name="box"/>.
        /// </summary>
        /// <param name="payloadType"></param>
        /// <param name="payload"></param>
        /// <param name="box"></param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool TryBox(Type payloadType, object payload, out NSObject box)
        {
            var boxer = LookupBoxer(payloadType);
            return boxer.TryBox(payload, out box);
        }

        /// <summary>
        /// Try to box a <paramref name="payload"/> of type <typeparamref name="T"/> into an NSObject <paramref name="box"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="box"></param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool TryBox<T>(T payload, out NSObject box)
        {
            return TryBox(typeof(T), payload, out box);
        }

        /// <summary>
        /// Try to unbox a <paramref name="payload"/> of type <paramref name="payloadType"/> from an NSObject <paramref name="box"/>.
        /// The caller must cast the <paramref name="payload"/> from object to the desired type.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="payloadType"></param>
        /// <param name="payload">This is passed back as an object, but is actually of type <paramref name="payloadType"/>.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool TryUnbox(NSObject box, Type payloadType, out object payload)
        {
            var boxer = LookupBoxer(payloadType);
            return boxer.TryUnbox(box, out payload);
        }

        /// <summary>
        /// Try to unbox a <paramref name="payload"/> of type <typeparamref name="T"/> from an NSObject <paramref name="box"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="box"></param>
        /// <param name="payload"></param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool TryUnbox<T>(NSObject box, out T payload)
        {
            if (TryUnbox(box, typeof(T), out object payloadObj))
            {
                payload = (T)payloadObj;
                return true;
            }
            else
            {
                payload = default;
                return false;
            }
        }

        /// <summary>
        /// Base class for all types of boxers.
        /// </summary>
        public abstract class Boxer
        {
            /// <summary>
            /// The type of payload processed by this boxer.
            /// </summary>
            public abstract Type PayloadType { get; }

            /// <summary>
            /// The type of box used by this boxer.
            /// </summary>
            public abstract Type BoxType { get; }

            /// <summary>
            /// Box the <paramref name="payload"/> into a box of type NSObject.
            /// </summary>
            /// <param name="payload"></param>
            /// <returns>The boxed <paramref name="payload"/>.</returns>
            /// <exception cref="InvalidOperationException">Thrown if the specified payload cannot be handled by this boxer.</exception>
            public abstract NSObject Box(object payload);

            /// <summary>
            /// Try to box the <paramref name="payload"/> into a <paramref name="box"/> of type NSObject.
            /// </summary>
            /// <param name="payload"></param>
            /// <param name="box"></param>
            /// <returns>True if successful; false otherwise.</returns>
            public abstract bool TryBox(object payload, out NSObject box);

            /// <summary>
            /// Try to unbox the <paramref name="payload"/> from an NSObject <paramref name="box"/>.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="payload"></param>
            /// <returns>True if successful; false otherwise.</returns>
            public abstract bool TryUnbox(NSObject box, out object payload);
        }

        /// <summary>
        /// Look up (or create) a boxer for the specified <paramref name="payloadType"/>.
        /// </summary>
        /// <param name="payloadType"></param>
        /// <returns>The boxer for the specified <paramref name="payloadType"/>.</returns>
        /// <exception cref="NotImplementedException">If no matching boxer exists then this exception is thrown.</exception>
        public static Boxer LookupBoxer(Type payloadType)
        {
            Boxer boxer;
            if (_boxerDictionary.TryGetValue(payloadType, out boxer))
            {
                return boxer;
            }
            else if (payloadType.IsSubclassOf(typeof(NSObject)))
            {
                // Create and cache these on demand.
                if (_onDemandBoxerDictionary.TryGetValue(payloadType, out boxer))
                {
                    return boxer;
                }
                else
                {
                    boxer = new BoxerNSObjectSubclass(payloadType);
                    _onDemandBoxerDictionary.TryAdd(payloadType, boxer);
                    return boxer;
                }
            }
            else
            {
                throw new NotImplementedException($"Boxing of type {payloadType.FullName} is not implemented");
            }
        }

        /// <summary>
        /// Look up (or create) a boxer for the specified payload type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The boxer for the specified payload type <typeparamref name="T"/>.</returns>
        /// <exception cref="NotImplementedException">If no matching boxer exists then this exception is thrown.</exception>
        public static Boxer LookupBoxer<T>()
        {
            return LookupBoxer(typeof(T));
        }

        private static readonly Dictionary<Type, Boxer> _boxerDictionary = new Dictionary<Type, Boxer>
        {
            { typeof(NSObject), new BoxerNSObject() },
            { typeof(string),   new BoxerString() },
            { typeof(bool),     new BoxerNumber<bool>(payload => new NSNumber(payload), box => (bool)box) },
            { typeof(Byte),     new BoxerNumber<Byte>(payload => new NSNumber(payload), box => (Byte)box) },
            { typeof(SByte),    new BoxerNumber<SByte>(payload => new NSNumber(payload), box => (SByte)box) },
            { typeof(Int16),    new BoxerNumber<Int16>(payload => new NSNumber(payload), box => (Int16)box) },
            { typeof(UInt16),   new BoxerNumber<UInt16>(payload => new NSNumber(payload), box => (UInt16)box) },
            { typeof(Int32),    new BoxerNumber<Int32>(payload => new NSNumber(payload), box => (Int32)box) },
            { typeof(UInt32),   new BoxerNumber<UInt32>(payload => new NSNumber(payload), box => (UInt32)box) },
            { typeof(Int64),    new BoxerNumber<Int64>(payload => new NSNumber(payload), box => (Int64)box) },
            { typeof(UInt64),   new BoxerNumber<UInt64>(payload => new NSNumber(payload), box => (UInt64)box) },
            { typeof(Single),   new BoxerNumber<Single>(payload => new NSNumber(payload), box => (Single)box) },
            { typeof(Double),   new BoxerNumber<Double>(payload => new NSNumber(payload), box => (Double)box) },
        };

        private static ConcurrentDictionary<Type, Boxer> _onDemandBoxerDictionary = new ConcurrentDictionary<Type, Boxer>();

        /// <summary>
        /// Boxer for NSObjects.
        /// </summary>
        public class BoxerNSObject : Boxer
        {
            // NSObject doesn't need to be boxed so the payload and box types are one and the same.
            public override Type PayloadType => typeof(NSObject);
            public override Type BoxType => typeof(NSObject);

            public override NSObject Box(object payload)
            {
                if (TryBox(payload, out var box))
                {
                    return box;
                }
                else
                {
                    throw new InvalidOperationException("Payload must be of type NSObject or null.");
                }
            }

            public override bool TryBox(object payload, out NSObject box)
            {
                if (payload is NSObject nsObject)
                {
                    // Box is already NSObject so just use it as is.
                    box = nsObject;
                    return true;
                }
                else if (payload == null)
                {
                    box = new NSNull();
                    return true;
                }
                else
                {
                    box = default;
                    return false;
                }
            }

            public override bool TryUnbox(NSObject box, out object payload)
            {
                if (box != default)
                {
                    payload = box.Is<NSNull>() ? null : box;
                    return true;
                }
                else
                {
                    payload = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Boxer for subclasses of NSObject.
        /// </summary>
        public class BoxerNSObjectSubclass : BoxerNSObject
        {
            public BoxerNSObjectSubclass(Type type)
            {
                // NSObject subclasses don't need to be boxed so the payload and box types are one and the same.
                PayloadType = type;
                BoxType = type;
            }

            public override Type PayloadType { get; }
            public override Type BoxType { get; }

            public override bool TryUnbox(NSObject box, out object payload)
            {
                if (Equals(box?.GetType(), PayloadType))
                {
                    // Box is already of the desired type so use it as is.
                    payload = box;
                    return true;
                }
                else if (base.TryUnbox(box, out var nsObject))
                {
                    payload = ((NSObject)nsObject)?.As(PayloadType);
                    return true;
                }
                else
                {
                    payload = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Boxer for subclasses of NSObject.
        /// </summary>
        /// <typeparam name="T">The subclass of NSObject handled by this boxer.</typeparam>
        public class BoxerNSObjectSubclass<T> : BoxerNSObjectSubclass where T : NSObject
        {
            public BoxerNSObjectSubclass() : base(typeof(T))
            {
            }

            public bool TryUnbox(NSObject box, out T payload)
            {
                if (base.TryUnbox(box, out var nsObjectSubclass))
                {
                    payload = (T)nsObjectSubclass;
                    return true;
                }
                else
                {
                    payload = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Boxer for C# string types.
        /// </summary>
        public class BoxerString : Boxer
        {
            private static readonly BoxerNSObjectSubclass<NSString> _boxerNSString = new BoxerNSObjectSubclass<NSString>();

            public override Type PayloadType => typeof(string);
            public override Type BoxType => typeof(NSString);

            public override NSObject Box(object payload)
            {
                if (TryBox(payload, out var box))
                {
                    return box;
                }
                else
                {
                    throw new InvalidOperationException("Payload must be a string or null.");
                }
            }

            public override bool TryBox(object payload, out NSObject box)
            {
                if (payload is string s)
                {
                    box = new NSString(s);
                    return true;
                }
                else if (payload == null)
                {
                    box = new NSNull();
                    return true;
                }
                else
                {
                    box = default;
                    return false;
                }
            }

            public override bool TryUnbox(NSObject box, out object payload)
            {
                if (_boxerNSString.TryUnbox(box, out var nsString))
                {
                    payload = nsString?.ToString();
                    return true;
                }
                else
                {
                    payload = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Boxer for numbers of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BoxerNumber<T> : Boxer
        {
            private static readonly BoxerNSObjectSubclass<NSNumber> _boxerNSNumber = new BoxerNSObjectSubclass<NSNumber>();

            public BoxerNumber(Func<T, NSNumber> factory, Func<NSNumber, T> cast)
            {
                Factory = factory;
                Cast = cast;
            }

            public override Type PayloadType => typeof(T);
            public override Type BoxType => typeof(NSNumber);

            private Func<T, NSNumber> Factory { get; }
            private Func<NSNumber, T> Cast { get; }

            public override NSObject Box(object payload)
            {
                if (TryBox(payload, out var box))
                {
                    return box;
                }
                else
                {
                    throw new InvalidOperationException($"Payload must be one of the supported number types.");
                }
            }

            public override bool TryBox(object payload, out NSObject box)
            {
                if (payload is T value)
                {
                    return _boxerNSNumber.TryBox(Factory(value), out box);
                }
                else
                {
                    box = default;
                    return false;
                }
            }

            public override bool TryUnbox(NSObject box, out object payload)
            {
                if (_boxerNSNumber.TryUnbox(box, out NSNumber nsNumber)
                    && nsNumber != null)
                {
                    payload = Cast(nsNumber);
                    return payload != null;
                }
                else
                {
                    payload = default;
                    return false;
                }
            }
        }
    }
}
