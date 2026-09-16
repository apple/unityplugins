using UnityEngine;

namespace Apple.Core
{
    /// <summary>
    /// Version-agnostic access to a Unity object's identity.
    ///
    /// Unity 6000.4 deprecated Object.GetInstanceID() in favour of GetEntityId(); from 2022.3
    /// through 6000.3 the int instance ID is still the current, non-obsolete API. This helper picks
    /// whichever is right for the editor being compiled against, so call sites don't have to.
    ///
    /// Deliberately exposes no EntityId type of its own: UnityEngine.EntityId exists from Unity 6
    /// onward, and a second one in this namespace would make every file importing both ambiguous.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// The object's identity widened to 64 bits, for passing across the native boundary.
        ///
        /// Treat the result as an opaque, process-local handle. The two branches below draw from
        /// different number spaces — a genuine 64-bit entity ID on 6000.4 and newer, a sign-extended
        /// 32-bit instance ID before that — so a value must never be serialized, nor compared
        /// against one produced by a different editor version.
        /// </summary>
        public static ulong GetLongId(this Object @object)
        {
#if UNITY_6000_4_OR_NEWER
            return EntityId.ToULong(@object.GetEntityId());
#else
            return (ulong)@object.GetInstanceID();
#endif
        }
    }
}
