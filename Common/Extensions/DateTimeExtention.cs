using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions
{
    public static class DateTimeExtention
    {
        public static DateTime KeepOffsetTo(this DateTimeOffset dateTime)
        {
            return DateTimeUtility.KeepOffsetTo(dateTime);
        }
    }
}
