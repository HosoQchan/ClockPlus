using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ClockPlus
{
    /* 切り上げ
    * 
    // 切り上げた桁未満は 0 埋めになる
    var d = new DateTime(2021, 11, 11, 11, 11, 11).AddTicks(1111111);
                              // 2021-11-11 11:11:11.1111111
    d.RoundUpToDay();         // 2021-11-12 00:00:00.0000000
    d.RoundUpToHour();        // 2021-11-11 12:00:00.0000000
    d.RoundUpToMinute();      // 2021-11-11 11:12:00.0000000
    d.RoundUpToSecond();      // 2021-11-11 11:11:12.0000000
    d.RoundUpToMillisecond(); // 2021-11-11 11:11:11.1120000
    d.RoundUpToMicrosecond(); // 2021-11-11 11:11:11.1111120
    */

    /* 切り捨て
     * 
    // 切り捨てた桁未満は 0 になる
    var d = new DateTime(2021, 11, 11, 11, 11, 11).AddTicks(1111111);
                                // 2021-11-11 11:11:11.1111111
    d.RoundDownToDay();         // 2021-11-11 00:00:00.0000000
    d.RoundDownToHour();        // 2021-11-11 11:00:00.0000000
    d.RoundDownToMinute();      // 2021-11-11 11:11:00.0000000
    d.RoundDownToSecond();      // 2021-11-11 11:11:11.0000000

    /* 四捨五入
     * 
    // 切り上げ
    var d = new DateTime(2021, 11, 11, 12, 30, 30).AddTicks(500_500_5);
                                        // 2021-11-11 12:30:30.5005005
    d.RoundAwayFromZeroToDay();         // 2021-11-12 00:00:00.0000000
    d.RoundAwayFromZeroToHour();        // 2021-11-11 13:00:00.0000000
    d.RoundAwayFromZeroToMinute();      // 2021-11-11 12:31:00.0000000

    // 切り捨て
    var d = new DateTime(2021, 11, 11, 11, 29, 29).AddTicks(499_499_4);
                                        // 2021-11-11 11:29:29.4994994
    d.RoundAwayFromZeroToDay();         // 2021-11-11 00:00:00.0000000
    d.RoundAwayFromZeroToHour();        // 2021-11-11 11:00:00.0000000
    d.RoundAwayFromZeroToMinute();      // 2021-11-11 11:29:00.0000000
    d.RoundAwayFromZeroToSecond();      // 2021-11-11 11:29:29.0000000
    */
    static public class DateTimeExt
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

        public static DateTime RoundUpToDay(this DateTime value) => value.RoundUpCore(TimeSpan.TicksPerDay);
        public static DateTime RoundUpToHour(this DateTime value) => value.RoundUpCore(TimeSpan.TicksPerHour);
        public static DateTime RoundUpToMinute(this DateTime value) => value.RoundUpCore(TimeSpan.TicksPerMinute);
        public static DateTime RoundUpToSecond(this DateTime value) => value.RoundUpCore(TimeSpan.TicksPerSecond);
        public static DateTime RoundUpToMillisecond(this DateTime value) => value.RoundUpCore(TimeSpan.TicksPerMillisecond);
        public static DateTime RoundUpToMicrosecond(this DateTime value) => value.RoundUpCore(TicksPerMicrosecond);
        public static DateTime RoundUp(this DateTime value, TimeSpan interval) => value.RoundUpCore(interval.Ticks);

        public static DateTime RoundDownToDay(this DateTime value) => value.RoundDownCore(TimeSpan.TicksPerDay);
        public static DateTime RoundDownToHour(this DateTime value) => value.RoundDownCore(TimeSpan.TicksPerHour);
        public static DateTime RoundDownToMinute(this DateTime value) => value.RoundDownCore(TimeSpan.TicksPerMinute);
        public static DateTime RoundDownToSecond(this DateTime value) => value.RoundDownCore(TimeSpan.TicksPerSecond);
        public static DateTime RoundDownToMillisecond(this DateTime value) => value.RoundDownCore(TimeSpan.TicksPerMillisecond);
        public static DateTime RoundDownToMicrosecond(this DateTime value) => value.RoundDownCore(TicksPerMicrosecond);
        public static DateTime RoundDown(this DateTime value, TimeSpan interval) => value.RoundDownCore(interval.Ticks);

        public static DateTime RoundAwayFromZeroToDay(this DateTime value) => value.RoundAwayFromZeroCore(TimeSpan.TicksPerDay);
        public static DateTime RoundAwayFromZeroToHour(this DateTime value) => value.RoundAwayFromZeroCore(TimeSpan.TicksPerHour);
        public static DateTime RoundAwayFromZeroToMinute(this DateTime value) => value.RoundAwayFromZeroCore(TimeSpan.TicksPerMinute);
        public static DateTime RoundAwayFromZeroToSecond(this DateTime value) => value.RoundAwayFromZeroCore(TimeSpan.TicksPerSecond);
        public static DateTime RoundAwayFromZeroToMillisecond(this DateTime value) => value.RoundAwayFromZeroCore(TimeSpan.TicksPerMillisecond);
        public static DateTime RoundAwayFromZeroToMicrosecond(this DateTime value) => value.RoundAwayFromZeroCore(TicksPerMicrosecond);
        public static DateTime RoundAwayFromZero(this DateTime value, TimeSpan interval) => value.RoundAwayFromZeroCore(interval.Ticks);

        private static DateTime RoundUpCore(this DateTime value, long interval)
        {
            return value.Ticks % interval is var overflow and > 0
                ? value.AddTicks(interval - overflow)
                : value;
        }

        private static DateTime RoundDownCore(this DateTime value, long interval)
        {
            return value.AddTicks(-(value.Ticks % interval));
        }

        private static DateTime RoundAwayFromZeroCore(this DateTime value, long interval)
        {
            var ticks = value.Ticks + interval >> 1;
            return new DateTime(ticks - ticks % interval, value.Kind);
        }
    }
}
