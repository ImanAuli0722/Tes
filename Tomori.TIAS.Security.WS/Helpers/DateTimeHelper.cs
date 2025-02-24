using System;

namespace Tomori.TIAS.Security.WS.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetNow()
        {
            // Ambil waktu UTC (misalnya dari database atau sistem)
            DateTime utcNow = DateTime.UtcNow;

            // Zona waktu WIB
            TimeZoneInfo wibZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Konversi ke WIB
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, wibZone);
        }

        public static string Convert(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }
    }
}