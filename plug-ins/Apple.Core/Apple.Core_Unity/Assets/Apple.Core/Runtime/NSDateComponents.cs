using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// Partial C# wrapper around NSDateComponents.
    /// </summary>
    public class NSDateComponents : NSObject
    {
        public static readonly long NSDateComponentUndefined = long.MaxValue;

        /// <summary>
        /// Construct an NSDateComponents wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        public NSDateComponents(IntPtr pointer) : base(pointer)
        {
        }

        public long Era
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Era, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Era, NSException.ThrowOnExceptionCallback);
        }

        public long Year
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Year, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Year, NSException.ThrowOnExceptionCallback);
        }

        public long Month
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Month, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Month, NSException.ThrowOnExceptionCallback);
        }

        public long Day
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Day, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Day, NSException.ThrowOnExceptionCallback);
        }

        public long Hour
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Hour, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Hour, NSException.ThrowOnExceptionCallback);
        }

        public long Minute
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Minute, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Minute, NSException.ThrowOnExceptionCallback);
        }

        public long Second
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Second, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Second, NSException.ThrowOnExceptionCallback);
        }

        public long Nanosecond
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Nanosecond, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Nanosecond, NSException.ThrowOnExceptionCallback);
        }

        public long Weekday
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Weekday, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Weekday, NSException.ThrowOnExceptionCallback);
        }

        public long WeekdayOrdinal
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.WeekdayOrdinal, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.WeekdayOrdinal, NSException.ThrowOnExceptionCallback);
        }

        public long Quarter
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.Quarter, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.Quarter, NSException.ThrowOnExceptionCallback);
        }

        public long WeekOfMonth
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.WeekOfMonth, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.WeekOfMonth, NSException.ThrowOnExceptionCallback);
        }

        public long WeekOfYear
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.WeekOfYear, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.WeekOfYear, NSException.ThrowOnExceptionCallback);
        }

        public long YearForWeekOfYear
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.YearForWeekOfYear, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.YearForWeekOfYear, NSException.ThrowOnExceptionCallback);
        }

        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public long DayOfYear
        {
            get => Interop.NSDateComponents_GetValueForComponent(Pointer, NSCalendarUnit.DayOfYear, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetValueForComponent(Pointer, value, NSCalendarUnit.DayOfYear, NSException.ThrowOnExceptionCallback);
        }

        public bool LeapMonth
        {
            get => Interop.NSDateComponents_GetLeapMonth(Pointer, NSException.ThrowOnExceptionCallback);
            set => Interop.NSDateComponents_SetLeapMonth(Pointer, value, NSException.ThrowOnExceptionCallback);
        }

        /// <summary>
        /// The date calculated from the current components using the stored calendar.
        /// </summary>
        /// <remarks>
        /// This property has millisecond precision.
        /// </remarks>
        public DateTimeOffset Date => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.NSDateComponents_GetDate(Pointer, NSException.ThrowOnExceptionCallback));

        /// <summary>
        /// Sets a value for a given calendar unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public void SetValueForComponent(long value, NSCalendarUnit unit) => Interop.NSDateComponents_SetValueForComponent(Pointer, value, unit, NSException.ThrowOnExceptionCallback);

        /// <summary>
        /// Returns the value for a given calendar unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public long GetValueForComponent(NSCalendarUnit unit) => Interop.NSDateComponents_GetValueForComponent(Pointer, unit, NSException.ThrowOnExceptionCallback);

        /// <summary>
        /// A Boolean value that indicates whether the current combination of properties represents a date which exists in the current calendar.
        /// </summary>
        public bool ValidDate => Interop.NSDateComponents_ValidDate(Pointer, NSException.ThrowOnExceptionCallback);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)] public static extern void NSDateComponents_SetLeapMonth(IntPtr thisPtr, bool value, NSExceptionCallback onException);
            [DllImport(InteropUtility.DLLName)] public static extern bool NSDateComponents_GetLeapMonth(IntPtr thisPtr, NSExceptionCallback onException); 
            [DllImport(InteropUtility.DLLName)] public static extern double NSDateComponents_GetDate(IntPtr thisPtr, NSExceptionCallback onException); 
            [DllImport(InteropUtility.DLLName)] public static extern void NSDateComponents_SetValueForComponent(IntPtr thisPtr, long value, NSCalendarUnit unit, NSExceptionCallback onException);
            [DllImport(InteropUtility.DLLName)] public static extern long NSDateComponents_GetValueForComponent(IntPtr thisPtr, NSCalendarUnit unit, NSExceptionCallback onException); 
            [DllImport(InteropUtility.DLLName)] public static extern bool NSDateComponents_ValidDate(IntPtr thisPtr, NSExceptionCallback onException);
        }
    }
}
