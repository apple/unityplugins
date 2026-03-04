namespace Apple.Core.Runtime
{
    public enum CFCalendarUnit : ulong
    {
        kCFCalendarUnitEra = (1UL << 1),
        kCFCalendarUnitYear = (1UL << 2),
        kCFCalendarUnitMonth = (1UL << 3),
        kCFCalendarUnitDay = (1UL << 4),
        kCFCalendarUnitHour = (1UL << 5),
        kCFCalendarUnitMinute = (1UL << 6),
        kCFCalendarUnitSecond = (1UL << 7),
        kCFCalendarUnitWeekday = (1UL << 9),
        kCFCalendarUnitWeekdayOrdinal = (1UL << 10),
        kCFCalendarUnitQuarter = (1UL << 11),
        kCFCalendarUnitWeekOfMonth = (1UL << 12),
        kCFCalendarUnitWeekOfYear = (1UL << 13),
        kCFCalendarUnitYearForWeekOfYear = (1UL << 14),

        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        kCFCalendarUnitDayOfYear = (1UL << 16),
    };

    public enum NSCalendarUnit : ulong
    {
        Era                = CFCalendarUnit.kCFCalendarUnitEra,
        Year               = CFCalendarUnit.kCFCalendarUnitYear,
        Month              = CFCalendarUnit.kCFCalendarUnitMonth,
        Day                = CFCalendarUnit.kCFCalendarUnitDay,
        Hour               = CFCalendarUnit.kCFCalendarUnitHour,
        Minute             = CFCalendarUnit.kCFCalendarUnitMinute,
        Second             = CFCalendarUnit.kCFCalendarUnitSecond,
        Weekday            = CFCalendarUnit.kCFCalendarUnitWeekday,
        WeekdayOrdinal     = CFCalendarUnit.kCFCalendarUnitWeekdayOrdinal,
        Quarter            = CFCalendarUnit.kCFCalendarUnitQuarter,
        WeekOfMonth        = CFCalendarUnit.kCFCalendarUnitWeekOfMonth,
        WeekOfYear         = CFCalendarUnit.kCFCalendarUnitWeekOfYear,
        YearForWeekOfYear  = CFCalendarUnit.kCFCalendarUnitYearForWeekOfYear,
        Nanosecond         = (1 << 15),

        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        DayOfYear          = CFCalendarUnit.kCFCalendarUnitDayOfYear,

        Calendar           = (1 << 20),
        TimeZone           = (1 << 21),
    }
}