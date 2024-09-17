namespace System
{
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Helper method to create a DateTimeOffset from a double-precision number of seconds since the Unix epoch (Jan 1 1970).
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset FromUnixTimeSeconds(double seconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(TimeSpan.FromSeconds(seconds).TotalMilliseconds));
        }
    }
}
