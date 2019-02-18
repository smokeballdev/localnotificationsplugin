using System;
using System.IO;
using System.Xml.Serialization;

namespace Plugin.LocalNotifications.Extensions
{
    public static class Extensions
    {
        public static long AsEpochMilliseconds(this DateTime dateTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }

        public static string Serialize(this object obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        public static T Deserialize<T>(this string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}